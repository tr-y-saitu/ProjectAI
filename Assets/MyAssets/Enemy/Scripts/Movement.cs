using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MimicSpace
{
    /// <summary>
    /// �ړ�����N���X
    /// </summary>
    public class Movement : MonoBehaviour
    {
        [Header("Controls")]
        [Tooltip("�n�ʂ���̖{�̂̍���")]
        [Range(0.5f, 5f)]
        public float height = 0.8f;         // Mimic �̍���
        public float speed = 5f;            // �ړ����x
        Vector3 velocity = Vector3.zero;    // ���݂̑��x
        public float velocityLerpCoef = 4f; // ���x��ԌW��
        Mimic myMimic;                      // Mimic �̎Q��

        /// <summary>
        /// �J�n���̏���
        /// </summary>
        private void Start()
        {
            myMimic = GetComponent<Mimic>(); // Mimic �R���|�[�l���g���擾
        }

        /// <summary>
        /// �X�V
        /// </summary>
        void Update()
        {
            // ���͂���Ɉړ����������肵�A���x���Ԃ���
            velocity = Vector3.Lerp(velocity, new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * speed, velocityLerpCoef * Time.deltaTime);

            // Mimic �̑��x���X�V���A�r�̔z�u���œK��
            myMimic.velocity = velocity;

            // �ʒu���X�V
            transform.position = transform.position + velocity * Time.deltaTime;

            // ���C�L���X�g���쐬
            RaycastHit hit;
            Vector3 destHeight = transform.position;

            // ���C�L���X�g���g�p���Ēn�ʂ̍������擾
            if (Physics.Raycast(transform.position + Vector3.up * 5f, -Vector3.up, out hit))
                destHeight = new Vector3(transform.position.x, hit.point.y + height, transform.position.z);

            // Mimic �̍������Ԃ��Ċ��炩�ɒn�ʂɍ��킹��
            transform.position = Vector3.Lerp(transform.position, destHeight, velocityLerpCoef * Time.deltaTime);

            transform.position = new Vector3(transform.position.x,1.0f, transform.position.z);
        }
    }
}
