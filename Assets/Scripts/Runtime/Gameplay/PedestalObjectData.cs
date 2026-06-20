using InSun.GameCore.SunnyInspector;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    [SunnySettings(MenuPath = "Pedestal Objects")]
    [CreateAssetMenu(
        menuName = "DoubleD/Very Serious Jam Game/Pedestal Object",
        fileName = "Data_PedestalObject"
    )]
    internal sealed class PedestalObjectData : ScriptableObject
    {
        [SerializeField]
        private PedestalObjectActor pedestalObjectPrefab;

        public PedestalObjectActor CreatePedestalObject(Vector3 position, Quaternion rotation, Transform parent)
        {
            var instance = Instantiate(pedestalObjectPrefab, position, rotation, parent);

            return instance;
        }
    }
}
