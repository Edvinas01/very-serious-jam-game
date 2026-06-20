using InSun.GameCore;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal sealed class TrophyZoneActor : MonoBehaviour
    {
        [SerializeField]
        private Vector2 spawnAreaSize = new(3f, 3f);

        [Min(0f)]
        [SerializeField]
        private float spawnRotation = 15f;

        private GameplaySystem gameplaySystem;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(spawnAreaSize.x, 0f, spawnAreaSize.y));
        }

        private void Awake()
        {
            gameplaySystem = Game.GetObject<GameplaySystem>();
        }

        private void Start()
        {
            foreach (var paintedObject in gameplaySystem.FullyPaintedObjects)
            {
                paintedObject.IsKinematic = false;
                paintedObject.transform.position = GetRandomSpawnPoint();
                paintedObject.transform.rotation = Quaternion.Euler(
                    Random.Range(-spawnRotation, spawnRotation),
                    Random.Range(0f, 360f),
                    Random.Range(-spawnRotation, spawnRotation)
                );

                paintedObject.transform.parent = transform;
                paintedObject.gameObject.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            foreach (var paintedObject in gameplaySystem.FullyPaintedObjects)
            {
                gameplaySystem.Return(paintedObject);
            }
        }

        private Vector3 GetRandomSpawnPoint()
        {
            var localPoint = new Vector3(
                Random.Range(-spawnAreaSize.x * 0.5f, spawnAreaSize.x * 0.5f),
                0f,
                Random.Range(-spawnAreaSize.y * 0.5f, spawnAreaSize.y * 0.5f)
            );

            return transform.TransformPoint(localPoint);
        }
    }
}
