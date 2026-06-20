using InSun.GameCore;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    [SelectionBase]
    internal sealed class PedestalActor : MonoBehaviour
    {
        [SerializeField]
        private Transform objectParent;

        public Transform ObjectParent => objectParent;

        private void OnEnable()
        {
            Game.AddObject<int, PedestalActor>(GetInstanceID(), this);
        }

        private void OnDisable()
        {
            Game.RemoveObject<int, PedestalActor>(GetInstanceID());
        }
    }
}
