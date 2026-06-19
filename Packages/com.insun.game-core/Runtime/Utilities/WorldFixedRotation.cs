using UnityEngine;

namespace InSun.GameCore.Utilities
{
    public sealed class WorldFixedRotation : MonoBehaviour
    {
        [SerializeField]
        private Vector3 forwardAxis =  Vector3.forward;

        private void LateUpdate()
        {
            transform.forward = forwardAxis;
        }
    }
}
