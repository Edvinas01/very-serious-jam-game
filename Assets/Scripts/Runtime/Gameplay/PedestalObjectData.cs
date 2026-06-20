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
        [Header("Info")]
        [SerializeField]
        private string objectName;

        [Header("Instantiation")]
        [SerializeField]
        private PedestalObjectActor pedestalObjectPrefab;

        [Header("Scoring")]
        [Min(0)]
        [SerializeField]
        private int score = 150;

        [SerializeField]
        private Vector2 fullyPaintedRange = new(0f, 0.8f);

        [Header("Animations")]
        [SerializeField]
        private AnimationClip slideInClip;

        [SerializeField]
        private AnimationClip slideOutClip;

        public string Name => objectName;

        public int Score => score;

        public Vector2 FullyPaintedRange => fullyPaintedRange;

        public AnimationClip SlideInClip => slideInClip;

        public AnimationClip SlideOutClip => slideOutClip;

        public PedestalObjectActor CreatePedestalObject(Vector3 position, Quaternion rotation, Transform parent)
        {
            var instance = Instantiate(pedestalObjectPrefab, position, rotation, parent);
            instance.name = $"{nameof(PedestalObjectActor)} ({Name})";
            instance.Data = this;

            return instance;
        }
    }
}
