using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalSpace
{
    /// <summary>
    /// 移動制御
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class ObjectMover : MonoBehaviour

    {
        [Header("Controls")]
        [Tooltip("地面からの本体の高さ")]
        [Range(0.5f, 5f)]
        public float height = 0.8f;         // オブジェクトの高さ
        public float speed = 5f;            // 移動速度
        public float velocityLerpCoef = 4f; // 速度補間係数

        private Vector3 velocity = Vector3.zero; // 現在の速度
        private Rigidbody rb;

        /// <summary>
        /// 開始時の処理
        /// </summary>
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false; // 高さを手動調整するため、重力をオフに
            rb.constraints = RigidbodyConstraints.FreezeRotation; // 回転を固定
        }

        /// <summary>
        /// 更新
        /// </summary>
        void Update()
        {
            // 入力を基に移動方向を決定し、速度を補間する
            Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            velocity = Vector3.Lerp(velocity, inputDir * speed, velocityLerpCoef * Time.deltaTime);

            // Rigidbody を使って移動
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);

            // レイキャストを作成して地面の高さを取得
            AdjustHeight();
        }

        /// <summary>
        /// レイキャストで地面の高さを調整
        /// </summary>
        private void AdjustHeight()
        {
            RaycastHit hit;
            Vector3 destHeight = transform.position;

            // 地面の高さを取得
            if (Physics.Raycast(transform.position + Vector3.up * 5f, Vector3.down, out hit))
            {
                destHeight.y = hit.point.y + height;
            }

            // 高さをスムーズに補間
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, destHeight.y, transform.position.z), velocityLerpCoef * Time.deltaTime);
        }
    }
}
