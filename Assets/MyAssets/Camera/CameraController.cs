using UnityEngine;

namespace UniversalSpace
{
    public class CameraController : MonoBehaviour
    {
        public enum CameraMode { FPS, TPS }

        [Header("�J�����ݒ�")]
        [SerializeField] private CameraMode currentMode = CameraMode.TPS; // FPS/TPS�؂�ւ�
        [SerializeField] private Transform target;  // �v���C���[��Transform
        [SerializeField] private Vector3 tpsOffset = new Vector3(0, 1.5f, -3f); // TPS���̃J�����ʒu
        [SerializeField] private Vector3 fpsOffset = new Vector3(0, 1.5f, 0.2f); // FPS���̃J�����ʒu
        [SerializeField] private float mouseSensitivity = 3f; // �}�E�X���x
        [SerializeField] private float minYAngle = -30f;
        [SerializeField] private float maxYAngle = 60f;
        [SerializeField] private float tpsDistance = 3f; // TPS���̃J��������
        [SerializeField] private float collisionOffset = 0.2f; // �J�����R���W��������
        [SerializeField] private KeyCode switchKey = KeyCode.F; // FPS/TPS�؂�ւ��L�[

        private float yaw = 0f;
        private float pitch = 0f;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            // ���[�h�؂�ւ�
            if (Input.GetKeyDown(switchKey))
            {
                currentMode = (currentMode == CameraMode.FPS) ? CameraMode.TPS : CameraMode.FPS;
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // �}�E�X���͂ŃJ������]
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

            if (currentMode == CameraMode.TPS)
            {
                // TPS���[�h�̃J�����ʒu
                Vector3 desiredPosition = target.position + rotation * tpsOffset.normalized * tpsDistance;

                // �J�����R���W��������
                RaycastHit hit;
                if (Physics.Raycast(target.position, (desiredPosition - target.position).normalized, out hit, tpsDistance))
                {
                    desiredPosition = hit.point + hit.normal * collisionOffset;
                }

                transform.position = desiredPosition;
            }
            else
            {
                // FPS���[�h�̃J�����ʒu�i�v���C���[�ɌŒ�j
                transform.position = target.position + rotation * fpsOffset;
            }

            transform.LookAt(target.position + Vector3.up * fpsOffset.y);
        }
    }
}
