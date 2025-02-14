using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MimicSpace
{
    /// <summary>
    /// �~�~�b�N�G�l�~�[
    /// </summary>
    public class Mimic : MonoBehaviour
    {
        [Header("Animation")]
        public GameObject legPrefab; // ���̃v���n�u

        [Range(2, 20)]
        public int numberOfLegs = 5; // �~�~�b�N�������̐�
        [Tooltip("The number of splines per leg")]
        [Range(1, 10)]
        public int partsPerLeg = 4; // 1�{�̑������p�[�c�̐�
        int maxLegs; // �ő呫��

        public int legCount; // ���݂̑��̐�
        public int deployedLegs; // �z�u�ς݂̑��̐�
        [Range(0, 19)]
        public int minimumAnchoredLegs = 2; // �Œ���n�ʂɌŒ肳��鑫�̐�
        public int minimumAnchoredParts; // �Œ���Œ肳��鑫�p�[�c�̐�

        [Tooltip("Minimum duration before leg is replaced")]
        public float minLegLifetime = 5; // ���̍ŒZ��������
        [Tooltip("Maximum duration before leg is replaced")]
        public float maxLegLifetime = 15; // ���̍Œ���������

        public Vector3 legPlacerOrigin = Vector3.zero; // �V��������z�u�����_
        [Tooltip("Leg placement radius offset")]
        public float newLegRadius = 3; // �V�������̔z�u���a�I�t�Z�b�g

        public float minLegDistance = 4.5f; // ����z�u����ŏ�����
        public float maxLegDistance = 6.3f; // ����z�u����ő勗��

        [Range(2, 50)]
        [Tooltip("Number of spline samples per legpart")]
        public int legResolution = 40; // 1�̑��p�[�c������̃X�v���C���T���v����

        [Tooltip("Minimum lerp coeficient for leg growth smoothing")]
        public float minGrowCoef = 4.5f; // ���̐�����Ԃ̍ŏ��W��
        [Tooltip("Maximum lerp coeficient for leg growth smoothing")]
        public float maxGrowCoef = 6.5f; // ���̐�����Ԃ̍ő�W��

        [Tooltip("Minimum duration before a new leg can be placed")]
        public float newLegCooldown = 0.3f; // �V��������ݒu����܂ł̃N�[���_�E������

        bool canCreateLeg = true; // ���𐶐��ł��邩�ǂ����̃t���O

        List<GameObject> availableLegPool = new List<GameObject>(); // ���̃I�u�W�F�N�g�v�[��

        [Tooltip("This must be updated as the Mimic moves to assure great leg placement")]
        public Vector3 velocity; // �~�~�b�N�̈ړ��x�N�g��

        void Start()
        {
            ResetMimic(); // ������
        }

        private void OnValidate()
        {
            ResetMimic(); // �C���X�y�N�^�[�̒l���ύX���ꂽ�Ƃ��Ƀ��Z�b�g
        }

        private void ResetMimic()
        {
            // �����̑������ׂč폜
            foreach (Leg g in GameObject.FindObjectsOfType<Leg>())
            {
                Destroy(g.gameObject);
            }
            legCount = 0;
            deployedLegs = 0;

            maxLegs = numberOfLegs * partsPerLeg; // �ő呫���̌v�Z
            float rot = 360f / maxLegs; // �p�x�v�Z�i���g�p�j
            Vector2 randV = Random.insideUnitCircle; // �����_���ȕ������擾
            velocity = new Vector3(randV.x, 0, randV.y); // �������x��ݒ�
            minimumAnchoredParts = minimumAnchoredLegs * partsPerLeg; // �Œ���Œ肳���p�[�c���̌v�Z
            maxLegDistance = newLegRadius * 2.1f; // �ő呫�����̌v�Z
        }

        IEnumerator NewLegCooldown()
        {
            canCreateLeg = false;
            yield return new WaitForSeconds(newLegCooldown);
            canCreateLeg = true;
        }

        // ���t���[���X�V
        void Update()
        {
            if (!canCreateLeg)
                return;

            // �V�������̔z�u��_���~�~�b�N�̐i�s�����ɐݒ�
            legPlacerOrigin = transform.position + velocity.normalized * newLegRadius;

            if (legCount <= maxLegs - partsPerLeg)
            {
                // �V�������̈ʒu�������_���ɃI�t�Z�b�g
                Vector2 offset = Random.insideUnitCircle * newLegRadius;
                Vector3 newLegPosition = legPlacerOrigin + new Vector3(offset.x, 0, offset.y);

                // �����~�~�b�N�������Ă��āA�V�������̈ʒu�����Ȃ�ΑO���ɏC��
                if (velocity.magnitude > 1f)
                {
                    float newLegAngle = Vector3.Angle(velocity, newLegPosition - transform.position);
                    if (Mathf.Abs(newLegAngle) > 90)
                    {
                        newLegPosition = transform.position - (newLegPosition - transform.position);
                    }
                }

                // �ŏ������ȉ��̏ꍇ�͓K�؂ȋ����ɕ␳
                if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(legPlacerOrigin.x, 0, legPlacerOrigin.z)) < minLegDistance)
                    newLegPosition = ((newLegPosition - transform.position).normalized * minLegDistance) + transform.position;

                // �p�x���傫������ꍇ�A���̈ʒu�𒲐�
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

        // �I�u�W�F�N�g�v�[�������p���đ��𐶐�
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

        // �������T�C�N���i�I�u�W�F�N�g�v�[���ɖ߂��j
        public void RecycleLeg(GameObject leg)
        {
            availableLegPool.Add(leg);
            leg.SetActive(false);
        }
    }
}
