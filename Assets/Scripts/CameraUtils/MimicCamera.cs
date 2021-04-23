using UnityEngine;

namespace CameraUtils
{
    public class MimicCamera : MonoBehaviour
    {
        [SerializeField] private Camera toMimic;
        private Camera _myCamera;

        private void Awake()
        {
            _myCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            _myCamera.orthographic = toMimic.orthographic;
            _myCamera.orthographicSize = toMimic.orthographicSize;
        }
    }
}