using UnityEngine;

namespace UniversalSpace
{
    public class CameraController : MonoBehaviour
    {
        public enum CameraMode { FPS, TPS }

        [Header("カメラ設定")]
        [SerializeField] private CameraMode currentMode = CameraMode.TPS; // FPS/TPS切り替え
        [SerializeField] private Transform target;  // プレイヤーのTransform
        [SerializeField] private Vector3 tpsOffset = new Vector3(0, 1.5f, -3f); // TPS時のカメラ位置
        [SerializeField] private Vector3 fpsOffset = new Vector3(0, 1.5f, 0.2f); // FPS時のカメラ位置
        [SerializeField] private float mouseSensitivity = 3f; // マウス感度
        [SerializeField] private float minYAngle = -30f;
        [SerializeField] private float maxYAngle = 60f;
        [SerializeField] private float tpsDistance = 3f; // TPS時のカメラ距離
        [SerializeField] private float collisionOffset = 0.2f; // カメラコリジョン調整
        [SerializeField] private KeyCode switchKey = KeyCode.F; // FPS/TPS切り替えキー

        private float yaw = 0f;
        private float pitch = 0f;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            // モード切り替え
            if (Input.GetKeyDown(switchKey))
            {
                currentMode = (currentMode == CameraMode.FPS) ? CameraMode.TPS : CameraMode.FPS;
            }
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // マウス入力でカメラ回転
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

            if (currentMode == CameraMode.TPS)
            {
                // TPSモードのカメラ位置
                Vector3 desiredPosition = target.position + rotation * tpsOffset.normalized * tpsDistance;

                // カメラコリジョン処理
                RaycastHit hit;
                if (Physics.Raycast(target.position, (desiredPosition - target.position).normalized, out hit, tpsDistance))
                {
                    desiredPosition = hit.point + hit.normal * collisionOffset;
                }

                transform.position = desiredPosition;
            }
            else
            {
                // FPSモードのカメラ位置（プレイヤーに固定）
                transform.position = target.position + rotation * fpsOffset;
            }

            transform.LookAt(target.position + Vector3.up * fpsOffset.y);
        }
    }
}
