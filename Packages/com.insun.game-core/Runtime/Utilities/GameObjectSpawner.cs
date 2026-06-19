using InSun.GameCore.Transforms;
using UnityEngine;

namespace InSun.GameCore.Utilities
{
    public sealed class GameObjectSpawner : MonoBehaviour
    {
        [Header("General")]
        [SerializeField]
        private GameObject gameObjectPrefab;

        [Header("Features")]
        [SerializeField]
        private string parentTransformName;

        [SerializeField]
        private bool isSpawnOnStart;

        [SerializeField]
        private bool isIgnoreRotation;

        private void Start()
        {
            if (isSpawnOnStart)
            {
                Spawn();
            }
        }

        public void Spawn()
        {
            if (isIgnoreRotation)
            {
                Instantiate(gameObjectPrefab, transform.position, Quaternion.identity, parent: GetParentTransform());
            }
            else
            {
                Instantiate(gameObjectPrefab, transform.position, transform.rotation, parent: GetParentTransform());
            }
        }

        private Transform GetParentTransform()
        {
            var transformSystem = Game.GetObject<TransformSystem>();
            return transformSystem.GetTransform(parentTransformName);
        }
    }
}
