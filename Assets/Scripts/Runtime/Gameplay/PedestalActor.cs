using InSun.GameCore;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    [SelectionBase]
    internal sealed class PedestalActor : MonoBehaviour
    {
        [SerializeField]
        private Transform objectParent;

        [SerializeField]
        private float constantSpeed;

        [Min(0f)]
        [SerializeField]
        private float spinDecaySpeed = 0.3f;

        private float currentSpinSpeed;

        public Transform ObjectParent => objectParent;

        private void FixedUpdate()
        {
            var totalSpeed = constantSpeed + currentSpinSpeed;
            var deltaTime = Time.deltaTime;

            objectParent.Rotate(Vector3.up, totalSpeed * deltaTime, Space.World);
            currentSpinSpeed = Mathf.Lerp(currentSpinSpeed, 0f, spinDecaySpeed * deltaTime);
        }

        private void OnEnable()
        {
            Game.AddObject<int, PedestalActor>(GetInstanceID(), this);
        }

        private void OnDisable()
        {
            Game.RemoveObject<int, PedestalActor>(GetInstanceID());
        }

        public void AddSpinSpeed(float speed)
        {
            currentSpinSpeed += speed;
        }
    }
}
