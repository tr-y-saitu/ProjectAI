using UnityEngine;

namespace UniversalSpace
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMover : MonoBehaviour
    {
        [Header("移動設定")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private Transform cameraTransform; // カメラのTransform

        private CharacterController characterController;
        private Vector3 velocity;

        private void Start()
        {
            characterController = GetComponent<CharacterController>();

            // カメラTransformが未設定なら自動取得
            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            // 入力取得
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

            if (inputDir.magnitude >= 0.1f)
            {
                // カメラの方向を基準に移動方向を決定
                Vector3 camForward = cameraTransform.forward;
                Vector3 camRight = cameraTransform.right;

                // 水平方向だけ使用（Yは除去）
                camForward.y = 0f;
                camRight.y = 0f;
                camForward.Normalize();
                camRight.Normalize();

                Vector3 moveDir = camForward * inputDir.z + camRight * inputDir.x;
                characterController.Move(moveDir * moveSpeed * Time.deltaTime);

                // プレイヤーの向きを移動方向に合わせる
                if (moveDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(moveDir);
                }
            }

            // 重力処理
            if (characterController.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // 地面に吸着
            }

            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }
    }
}
