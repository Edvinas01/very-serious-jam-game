using InSun.GameCore.SunnyInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    [SunnySettings(MenuPath = "Paintables")]
    [CreateAssetMenu(
        menuName = "DoubleD/Very Serious Jam Game/Paintable Data",
        fileName = "Data_Paintable"
    )]
    internal sealed class PaintableData : ScriptableObject
    {
        [Header("Info")]
        [SerializeField]
        private string objectName;

        [SerializeField]
        private Sprite icon;

        [Header("Instantiation")]
        [FormerlySerializedAs("pedestalObjectPrefab")]
        [SerializeField]
        private PaintableActor paintablePrefab;

        // TODO
        [SerializeField]
        private Mesh paintableMesh;

        [SerializeField]
        private Texture2D paintableTexture;

        [SerializeField]
        private Material paintableMaterial;

        // TODO
        [Min(0)]
        [SerializeField]
        private Vector3 paintableScale = new(1f, 1f, 1f);

        [Min(0)]
        [SerializeField]
        private Vector3 paintableRotation = new(1f, 1f, 1f);

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

        public GameObject Prefab => paintablePrefab ? paintablePrefab.gameObject : null;

        public Sprite Icon => icon;

        public Mesh PaintableMesh => paintableMesh;

        public Texture2D PaintableTexture => paintableTexture;

        public Material PaintableMaterial => paintableMaterial;

        public Vector3 PaintableScale => paintableScale;

        public Vector3 PaintableRotation => paintableRotation;

        public int Score => score;

        public Vector2 FullyPaintedRange => fullyPaintedRange;

        public AnimationClip SlideInClip => slideInClip;

        public AnimationClip SlideOutClip => slideOutClip;

        public PaintableActor CreatePaintable(Vector3 position, Quaternion rotation, Transform parent)
        {
            var instance = Instantiate(paintablePrefab, position, rotation, parent);
            instance.name = $"{nameof(PaintableActor)} ({Name})";
            instance.Data = this;

            return instance;
        }
    }
}
