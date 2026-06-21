using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DoubleD.VerySeriousJamGame.Runtime.Utilities;
using InSun.GameCore;
using InSun.GameCore.Scenes;
using InSun.GameCore.Utilities;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    [SelectionBase]
    internal sealed class GameplayController : MonoBehaviour
    {
        [Header("Intro")]
        [SerializeField]
        private CinemachineCamera introCamera;

        [SerializeField]
        private PlayableDirector introPlayable;

        [Header("Data")]
        [SerializeField]
        private GameplayData data;

        private GameplaySystem gameplaySystem;
        private ISceneSystem sceneSystem;
        private CancellationTokenSource gameplayCancellation;

        private PlayerActor player;
        private PedestalActor pedestal;

        private void Awake()
        {
            gameplaySystem = Game.GetObject<GameplaySystem>();
            sceneSystem = Game.GetObject<ISceneSystem>();

            player = FindAnyObjectByType<PlayerActor>();
            if (player == false)
            {
                Debug.LogError("No Player found in scene, gameplay will not start", this);
            }

            pedestal = FindAnyObjectByType<PedestalActor>();
            if (pedestal == false)
            {
                Debug.LogError("No Pedestal found in scene, gameplay will not start", this);
            }
        }

        private void Start()
        {
            gameplayCancellation = new CancellationTokenSource();
            StartGameAsync(gameplayCancellation.Token).Forget();
        }

        private void OnDestroy()
        {
            gameplayCancellation?.Cancel();
            gameplayCancellation?.Dispose();
            gameplayCancellation = null;
        }

        private async UniTaskVoid StartGameAsync(CancellationToken cancellationToken)
        {
            if (player == false)
            {
                Debug.LogError("Cannot start game, no player found", this);
                return;
            }

            if (pedestal == false)
            {
                Debug.LogError("Cannot start game, no pedestal found", this);
                return;
            }

            gameplaySystem.State = GameplayState.Introduction;

            // Init game
            gameplaySystem.ClearPaintedObjects();
            gameplaySystem.RemainingTime = data.GameplayDuration;
            gameplaySystem.Score = data.StartingScore;

            // Disable player so no movement during intro
            player.DisableInteraction();
            player.DisableCamera();

            // Intro anim
            await PlayIntroAsync(cancellationToken);

            // Enable player
            player.EnableInteraction();
            player.EnableCamera();

            // Spawn pedestal object
            gameplaySystem.State = GameplayState.SpawningObject;
            var paintable = CreatePaintable(pedestal.ObjectParent);
            paintable.gameObject.SetActive(false);
            paintable.OnPainted += OnObjectPainted;
            await paintable.SlideInAsync(cancellationToken);
            gameplaySystem.State = GameplayState.PaintingObject;

            // Game loop
            do
            {
                // Switch painted object
                if (paintable.PaintAmount >= 1f)
                {
                    gameplaySystem.Score += paintable.Data.Score;

                    // Slide out old object
                    gameplaySystem.State = GameplayState.SpawningObject;
                    await paintable.SlideOutAsync(cancellationToken);
                    paintable.OnPainted -= OnObjectPainted;
                    gameplaySystem.Store(paintable);

                    // GG: reached max score
                    if (gameplaySystem.Score >= data.MaxScore)
                    {
                        gameplaySystem.State = GameplayState.GameOver;
                        break;
                    }

                    // Slide in new object
                    paintable = CreatePaintable(pedestal.ObjectParent);
                    paintable.gameObject.SetActive(false);
                    paintable.OnPainted += OnObjectPainted;

                    gameplaySystem.PaintAmount = 0f;

                    await paintable.SlideInAsync(cancellationToken);
                    gameplaySystem.State = GameplayState.PaintingObject;
                }

                await UniTask.Yield(cancellationToken);
            } while (gameplaySystem.State != GameplayState.GameOver);

            LoadGameOverScene();
        }

        private void OnObjectPainted(float paintAmount)
        {
            gameplaySystem.PaintAmount = paintAmount;
        }

        private PaintableActor CreatePaintable(Transform parent)
        {
            if (data.Paintables.TryGetRandom(out var pedestalObject))
            {
                return pedestalObject.CreatePaintable(
                    parent.position,
                    Quaternion.identity,
                    parent
                );
            }

            throw new Exception("No pedestal object found");
        }

        private void LoadGameOverScene()
        {
            sceneSystem.LoadScene(new SceneLoadArgs(data.GameOverScene));
        }

        private async UniTask PlayIntroAsync(CancellationToken cancellationToken)
        {
            if (introPlayable)
            {
                introCamera.enabled = true;
                await introPlayable.PlayAsync(cancellationToken);
                introCamera.enabled = false;
            }
        }
    }
}
