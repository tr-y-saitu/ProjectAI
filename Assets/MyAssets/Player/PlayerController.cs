using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace UniversalSpace
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("移動設定")]
        [SerializeField,Header("移動速度")] private float moveSpeed = 5f;
        [SerializeField,Header("回転速度")] private float rotationSpeed = 10f;
        [SerializeField,Header("カメラのTransform")] private Transform cameraTransform; // カメラのTransform

        private CharacterController controller;
        private Vector3 moveDirection;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            MovePlayer();
        }

        private void MovePlayer()
        {
            float horizontal = Input.GetAxis("Horizontal"); // A,D or ←→
            float vertical = Input.GetAxis("Vertical");     // W,S or ↑↓

            // カメラの正面と右方向を取得
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            // Y軸の影響を除外（水平移動のみ）
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            // 入力方向をカメラ基準で計算
            moveDirection = (forward * vertical + right * horizontal).normalized * moveSpeed;

            // プレイヤーの向きを移動方向に回転
            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // CharacterControllerで移動
            controller.Move(moveDirection * Time.deltaTime);
        }
    }
}
