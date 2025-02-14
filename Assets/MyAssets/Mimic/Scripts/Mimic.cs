using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MimicSpace
{
    /// <summary>
    /// ミミックエネミー
    /// </summary>
    public class Mimic : MonoBehaviour
    {
        [Header("Animation")]
        public GameObject legPrefab; // 足のプレハブ

        [Range(2, 20)]
        public int numberOfLegs = 5; // ミミックが持つ足の数
        [Tooltip("The number of splines per leg")]
        [Range(1, 10)]
        public int partsPerLeg = 4; // 1本の足が持つパーツの数
        int maxLegs; // 最大足数

        public int legCount; // 現在の足の数
        public int deployedLegs; // 配置済みの足の数
        [Range(0, 19)]
        public int minimumAnchoredLegs = 2; // 最低限地面に固定される足の数
        public int minimumAnchoredParts; // 最低限固定される足パーツの数

        [Tooltip("Minimum duration before leg is replaced")]
        public float minLegLifetime = 5; // 足の最短存続時間
        [Tooltip("Maximum duration before leg is replaced")]
        public float maxLegLifetime = 15; // 足の最長存続時間

        public Vector3 legPlacerOrigin = Vector3.zero; // 新しい足を配置する基準点
        [Tooltip("Leg placement radius offset")]
        public float newLegRadius = 3; // 新しい足の配置半径オフセット

        public float minLegDistance = 4.5f; // 足を配置する最小距離
        public float maxLegDistance = 6.3f; // 足を配置する最大距離

        [Range(2, 50)]
        [Tooltip("Number of spline samples per legpart")]
        public int legResolution = 40; // 1つの足パーツあたりのスプラインサンプル数

        [Tooltip("Minimum lerp coeficient for leg growth smoothing")]
        public float minGrowCoef = 4.5f; // 足の成長補間の最小係数
        [Tooltip("Maximum lerp coeficient for leg growth smoothing")]
        public float maxGrowCoef = 6.5f; // 足の成長補間の最大係数

        [Tooltip("Minimum duration before a new leg can be placed")]
        public float newLegCooldown = 0.3f; // 新しい足を設置するまでのクールダウン時間

        bool canCreateLeg = true; // 足を生成できるかどうかのフラグ

        List<GameObject> availableLegPool = new List<GameObject>(); // 足のオブジェクトプール

        [Tooltip("This must be updated as the Mimic moves to assure great leg placement")]
        public Vector3 velocity; // ミミックの移動ベクトル

        void Start()
        {
            ResetMimic(); // 初期化
        }

        private void OnValidate()
        {
            ResetMimic(); // インスペクターの値が変更されたときにリセット
        }

        private void ResetMimic()
        {
            // 既存の足をすべて削除
            foreach (Leg g in GameObject.FindObjectsOfType<Leg>())
            {
                Destroy(g.gameObject);
            }
            legCount = 0;
            deployedLegs = 0;

            maxLegs = numberOfLegs * partsPerLeg; // 最大足数の計算
            float rot = 360f / maxLegs; // 角度計算（未使用）
            Vector2 randV = Random.insideUnitCircle; // ランダムな方向を取得
            velocity = new Vector3(randV.x, 0, randV.y); // 初期速度を設定
            minimumAnchoredParts = minimumAnchoredLegs * partsPerLeg; // 最低限固定されるパーツ数の計算
            maxLegDistance = newLegRadius * 2.1f; // 最大足距離の計算
        }

        IEnumerator NewLegCooldown()
        {
            canCreateLeg = false;
            yield return new WaitForSeconds(newLegCooldown);
            canCreateLeg = true;
        }

        // 毎フレーム更新
        void Update()
        {
            if (!canCreateLeg)
                return;

            // 新しい足の配置基準点をミミックの進行方向に設定
            legPlacerOrigin = transform.position + velocity.normalized * newLegRadius;

            if (legCount <= maxLegs - partsPerLeg)
            {
                // 新しい足の位置をランダムにオフセット
                Vector2 offset = Random.insideUnitCircle * newLegRadius;
                Vector3 newLegPosition = legPlacerOrigin + new Vector3(offset.x, 0, offset.y);

                // もしミミックが動いていて、新しい足の位置が後ろならば前方に修正
                if (velocity.magnitude > 1f)
                {
                    float newLegAngle = Vector3.Angle(velocity, newLegPosition - transform.position);
                    if (Mathf.Abs(newLegAngle) > 90)
                    {
                        newLegPosition = transform.position - (newLegPosition - transform.position);
                    }
                }

                // 最小距離以下の場合は適切な距離に補正
                if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(legPlacerOrigin.x, 0, legPlacerOrigin.z)) < minLegDistance)
                    newLegPosition = ((newLegPosition - transform.position).normalized * minLegDistance) + transform.position;

                // 角度が大きすぎる場合、足の位置を調整
                if (Vector3.Angle(velocity, newLegPosition - transform.position) > 45)
                    newLegPosition = transform.position + ((newLegPosition - transform.position) + velocity.normalized * (newLegPosition - transform.position).magnitude) / 2f;

                RaycastHit hit;
                Physics.Raycast(newLegPosition + Vector3.up * 10f, -Vector3.up, out hit);
                Vector3 myHit = hit.point;
                if (Physics.Linecast(transform.position, hit.point, out hit))
                    myHit = hit.point;

                float lifeTime = Random.Range(minLegLifetime, maxLegLifetime);

                StartCoroutine("NewLegCooldown");
                for (int i = 0; i < partsPerLeg; i++)
                {
                    RequestLeg(myHit, legResolution, maxLegDistance, Random.Range(minGrowCoef, maxGrowCoef), this, lifeTime);
                    if (legCount >= maxLegs)
                        return;
                }
            }
        }

        // オブジェクトプールを活用して足を生成
        void RequestLeg(Vector3 footPosition, int legResolution, float maxLegDistance, float growCoef, Mimic myMimic, float lifeTime)
        {
            GameObject newLeg;
            if (availableLegPool.Count > 0)
            {
                newLeg = availableLegPool[availableLegPool.Count - 1];
                availableLegPool.RemoveAt(availableLegPool.Count - 1);
            }
            else
            {
                newLeg = Instantiate(legPrefab, transform.position, Quaternion.identity);
            }
            newLeg.SetActive(true);
            newLeg.GetComponent<Leg>().Initialize(footPosition, legResolution, maxLegDistance, growCoef, myMimic, lifeTime);
            newLeg.transform.SetParent(myMimic.transform);
        }

        // 足をリサイクル（オブジェクトプールに戻す）
        public void RecycleLeg(GameObject leg)
        {
            availableLegPool.Add(leg);
            leg.SetActive(false);
        }
    }
}
