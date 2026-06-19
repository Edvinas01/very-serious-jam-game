using InSun.GameCore.SunnyInspector;
using InSun.GameCore.Utilities;
using UnityEngine;

namespace InSun.GameCore.Pooling
{
    [SunnySettings(MenuPath = "Pooling")]
    [CreateAssetMenu(
        menuName = MenuConstants.BaseAssetMenuName + "/Pooling/Pooled Object Event",
        fileName = MenuConstants.BaseAssetFileName + "Data_PooledObjectEvent"
    )]
    public sealed class ObjectPoolData : ScriptableObject, IObjectPool
    {
        [Label("Name")]
        [SerializeField]
        private string poolName;

        [Header("Instancing")]
        [SerializeField]
        private PooledGameObject prefab;

        [Header("Instances")]
        [Tooltip("Global count for instances of THIS event")]
        [Min(0)]
        [SerializeField]
        private int maxInstanceCount = 10;

        [Tooltip(
            ""
            + "If a spawner reaches the global limit, it will try to steal a random instance "
            + "instead of doing nothing"
        )]
        [SerializeField]
        private bool isStealing;

        [Min(0)]
        [SerializeField]
        private int preloadCount;

        [Header("Spawning")]
        [SerializeField]
        [Min(0f)]
        private float spawnCooldown;

        [SerializeField]
        [Range(0f, 1f)]
        private float spawnChance = 1f;

        [Header("Culling")]
        [SerializeField]
        private bool isFrustumCull;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf(nameof(isFrustumCull))]
#else
        [InSun.GameCore.SunnyInspector.ShowIf(nameof(isFrustumCull))]
#endif
        [SerializeField]
        [Min(0f)]
        private float objectSize = 1f;

        public string Name => $"{prefab.name}: {poolName}";

        public PoolId PoolId { get; private set; }

        public int MaxInstanceCount => maxInstanceCount;

        public bool IsStealing => isStealing;

        public int PreloadCount => preloadCount;

        public float SpawnCooldown => spawnCooldown;

        public float SpawnChance => spawnChance;

        public bool IsFrustumCull => isFrustumCull;

        public float ObjectSize => objectSize;

        private void OnEnable()
        {
            PoolId = new PoolId(GetInstanceID());
        }

        public IPooledObject Create(Transform parent)
        {
            return Instantiate(prefab, parent);
        }
    }
}
