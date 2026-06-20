using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal sealed class CrankActor : MonoBehaviour
    {
        [SerializeField]
        private Transform handleTransform;

        [SerializeField]
        private Rigidbody crankRigidbody;
    }
}
