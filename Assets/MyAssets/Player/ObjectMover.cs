using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UniversalSpace
{
    /// <summary>
    /// �ړ�����
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class ObjectMover : MonoBehaviour

    {
        [Header("Controls")]
        [Tooltip("�n�ʂ���̖{�̂̍���")]
        [Range(0.5f, 5f)]
        public float height = 0.8f;         // �I�u�W�F�N�g�̍���
        public float speed = 5f;            // �ړ����x
        public float velocityLerpCoef = 4f; // ���x��ԌW��

        private Vector3 velocity = Vector3.zero; // ���݂̑��x
        private Rigidbody rb;

        /// <summary>
        /// �J�n���̏���
        /// </summary>
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false; // �������蓮�������邽�߁A�d�͂��I�t��
            rb.constraints = RigidbodyConstraints.FreezeRotation; // ��]���Œ�
        }

        /// <summary>
        /// �X�V
        /// </summary>
        void Update()
        {
            // ���͂���Ɉړ����������肵�A���x���Ԃ���
            Vector3 inputDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            velocity = Vector3.Lerp(velocity, inputDir * speed, velocityLerpCoef * Time.deltaTime);

            // Rigidbody ���g���Ĉړ�
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);

            // ���C�L���X�g���쐬���Ēn�ʂ̍������擾
            AdjustHeight();
        }

        /// <summary>
        /// ���C�L���X�g�Œn�ʂ̍����𒲐�
        /// </summary>
        private void AdjustHeight()
        {
            RaycastHit hit;
            Vector3 destHeight = transform.position;

            // �n�ʂ̍������擾
            if (Physics.Raycast(transform.position + Vector3.up * 5f, Vector3.down, out hit))
            {
                destHeight.y = hit.point.y + height;
            }

            // �������X���[�Y�ɕ��
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, destHeight.y, transform.position.z), velocityLerpCoef * Time.deltaTime);
        }
    }
}
