using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    [SelectionBase]
    internal sealed class PedestalActor : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private PedestalData data;

        [Header("Paintables")]
        [SerializeField]
        private Transform objectParent;

        private float currentSpinSpeed;

        public float SpinSpeed => data.ConstantSpeed + currentSpinSpeed;

        public Transform ObjectParent => objectParent;

        private void FixedUpdate()
        {
            var totalSpeed = data.ConstantSpeed + currentSpinSpeed;
            var deltaTime = Time.deltaTime;

            objectParent.Rotate(Vector3.up, totalSpeed * deltaTime, Space.World);
            currentSpinSpeed = Mathf.Lerp(currentSpinSpeed, 0f, data.SpinDecaySpeed * deltaTime);
        }

        public void AddSpinSpeed(float speed)
        {
            currentSpinSpeed += speed;
        }
    }
}
