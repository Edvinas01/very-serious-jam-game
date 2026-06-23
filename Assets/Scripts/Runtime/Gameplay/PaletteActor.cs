using DoubleD.VerySeriousJamGame.Runtime.Utilities;
using InSun.GameCore.Animations;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal sealed class PaletteActor : MonoBehaviour
    {
        [SerializeField]
        private TransformMover asideMover;

        [SerializeField]
        private TweenAnimation showTween;

        public void MoveAside()
        {
            asideMover.MoveToTarget();
        }

        public void MoveBack()
        {
            asideMover.MoveBack();
        }
    }
}
