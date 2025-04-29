using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MimicSpace
{
    /// <summary>
    /// 移動制御クラス
    /// </summary>
    public class Movement : MonoBehaviour
    {
        [Header("Controls")]
        [Tooltip("地面からの本体の高さ")]
        [Range(0.5f, 5f)]
        public float height = 0.8f;         // Mimic の高さ
        public float speed = 5f;            // 移動速度
        Vector3 velocity = Vector3.zero;    // 現在の速度
        public float velocityLerpCoef = 4f; // 速度補間係数
        Mimic myMimic;                      // Mimic の参照

        /// <summary>
        /// 開始時の処理
        /// </summary>
        private void Start()
        {
            myMimic = GetComponent<Mimic>(); // Mimic コンポーネントを取得
        }

        /// <summary>
        /// 更新
        /// </summary>
        void Update()
        {
            // 入力を基に移動方向を決定し、速度を補間する
            velocity = Vector3.Lerp(velocity, new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * speed, velocityLerpCoef * Time.deltaTime);

            // Mimic の速度を更新し、脚の配置を最適化
            myMimic.velocity = velocity;

            // 位置を更新
            transform.position = transform.position + velocity * Time.deltaTime;

            // レイキャストを作成
            RaycastHit hit;
            Vector3 destHeight = transform.position;

            // レイキャストを使用して地面の高さを取得
            if (Physics.Raycast(transform.position + Vector3.up * 5f, -Vector3.up, out hit))
                destHeight = new Vector3(transform.position.x, hit.point.y + height, transform.position.z);

            // Mimic の高さを補間して滑らかに地面に合わせる
            transform.position = Vector3.Lerp(transform.position, destHeight, velocityLerpCoef * Time.deltaTime);

            transform.position = new Vector3(transform.position.x,1.0f, transform.position.z);
        }
    }
}
