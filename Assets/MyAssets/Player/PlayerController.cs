using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace UniversalSpace
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("�ړ��ݒ�")]
        [SerializeField,Header("�ړ����x")] private float moveSpeed = 5f;
        [SerializeField,Header("��]���x")] private float rotationSpeed = 10f;
        [SerializeField,Header("�J������Transform")] private Transform cameraTransform; // �J������Transform

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
            float horizontal = Input.GetAxis("Horizontal"); // A,D or ����
            float vertical = Input.GetAxis("Vertical");     // W,S or ����

            // �J�����̐��ʂƉE�������擾
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            // Y���̉e�������O�i�����ړ��̂݁j
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            // ���͕������J������Ōv�Z
            moveDirection = (forward * vertical + right * horizontal).normalized * moveSpeed;

            // �v���C���[�̌������ړ������ɉ�]
            if (moveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // CharacterController�ňړ�
            controller.Move(moveDirection * Time.deltaTime);
        }
    }
}
