using UnityEngine;

namespace InSun.GameCore.Utilities
{
    public sealed class Billboard : MonoBehaviour
    {
        [SerializeField]
        private bool isFlipped;

        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            var forward = mainCamera.transform.forward;
            if (isFlipped)
            {
                forward = -forward;
            }

            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }
    }
}
