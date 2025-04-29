using UnityEngine;

namespace UniversalSpace
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMover : MonoBehaviour
    {
        [Header("�ړ��ݒ�")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private Transform cameraTransform; // �J������Transform

        private CharacterController characterController;
        private Vector3 velocity;

        private void Start()
        {
            characterController = GetComponent<CharacterController>();

            // �J����Transform�����ݒ�Ȃ玩���擾
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
            // ���͎擾
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 inputDir = new Vector3(horizontal, 0f, vertical).normalized;

            if (inputDir.magnitude >= 0.1f)
            {
                // �J�����̕�������Ɉړ�����������
                Vector3 camForward = cameraTransform.forward;
                Vector3 camRight = cameraTransform.right;

                // �������������g�p�iY�͏����j
                camForward.y = 0f;
                camRight.y = 0f;
                camForward.Normalize();
                camRight.Normalize();

                Vector3 moveDir = camForward * inputDir.z + camRight * inputDir.x;
                characterController.Move(moveDir * moveSpeed * Time.deltaTime);

                // �v���C���[�̌������ړ������ɍ��킹��
                if (moveDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(moveDir);
                }
            }

            // �d�͏���
            if (characterController.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // �n�ʂɋz��
            }

            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }
    }
}
