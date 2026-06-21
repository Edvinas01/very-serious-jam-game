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

        [FormerlySerializedAs("paintableMesh")]
        [SerializeField]
        private Mesh sharedMesh;

        [FormerlySerializedAs("paintableTexture")]
        [SerializeField]
        private Texture2D texture;

        [FormerlySerializedAs("paintableMaterial")]
        [SerializeField]
        private Material material;

        [FormerlySerializedAs("paintableScale")]
        [Min(0)]
        [SerializeField]
        private Vector3 scale = new(1f, 1f, 1f);

        [FormerlySerializedAs("paintableRotation")]
        [Min(0)]
        [SerializeField]
        private Vector3 rotation = new(0f, 0f, 0f);

        [SerializeField]
        private Vector3 offset;

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

        public Mesh SharedMesh => sharedMesh;

        public Texture2D Texture => texture;

        public Material Material => material;

        public Vector3 Scale => scale;

        public Vector3 Rotation => rotation;

        public Vector3 Offset => offset;

        public int Score => score;

        public Vector2 FullyPaintedRange => fullyPaintedRange;

        public AnimationClip SlideInClip => slideInClip;

        public AnimationClip SlideOutClip => slideOutClip;

        public PaintableActor CreatePaintable(Vector3 pos, Quaternion rot, Transform parent)
        {
            var instance = Instantiate(paintablePrefab, pos, rot, parent);
            instance.Initialize(this);

            return instance;
        }
    }
}
