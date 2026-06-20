using InSun.GameCore;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal sealed class PedestalObjectActor : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer targetMesh;

        private void OnEnable()
        {
            Game.AddObject<int, PedestalObjectActor>(GetInstanceID(), this);
        }

        private void OnDisable()
        {
            Game.RemoveObject<int, PedestalObjectActor>(GetInstanceID());
        }
    }
}
