using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DoubleD.VerySeriousJamGame.Runtime.Utilities;
using InSun.GameCore;
using InSun.GameCore.Interactables;
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

        [Header("Multiplier")]
        [Min(0f)]
        [SerializeField]
        private float multiplierDecaySpeed = 0.5f;

        private GameplaySystem gameplaySystem;
        private ISceneSystem sceneSystem;
        private CancellationTokenSource gameplayCancellation;

        private HandActor hand;
        private PlayerActor player;
        private PedestalActor pedestal;
        private CrankActor crank;
        private PaletteActor palette;
        private List<PaintBrushActor> brushes;

        private int currentPaintableScore;
        private readonly List<PaintableData> paintableQueue = new();

        private void Awake()
        {
            gameplaySystem = Game.GetObject<GameplaySystem>();
            sceneSystem = Game.GetObject<ISceneSystem>();

            brushes = FindObjectsByType<PaintBrushActor>(FindObjectsSortMode.None).ToList();
            hand = FindAnyObjectByType<HandActor>();
            if (hand == false)
            {
                Debug.LogError("No Hand found in scene, gameplay will not start", this);
                enabled = false;
                return;
            }

            player = FindAnyObjectByType<PlayerActor>();
            if (player == false)
            {
                Debug.LogError("No Player found in scene, gameplay will not start", this);
                enabled = false;
                return;
            }

            pedestal = FindAnyObjectByType<PedestalActor>();
            if (pedestal == false)
            {
                Debug.LogError("No Pedestal found in scene, gameplay will not start", this);
                enabled = false;
                return;
            }

            crank = FindAnyObjectByType<CrankActor>();
            if (crank == false)
            {
                Debug.LogError("No Crank found in scene, gameplay will not start", this);
                enabled = false;
                return;
            }

            palette = FindAnyObjectByType<PaletteActor>();
            if (palette == false)
            {
                Debug.LogError("No Palette found in scene, gameplay will not start", this);
                enabled = false;
                return;
            }

            ReloadPaintableQueue();
        }

        private void OnEnable()
        {
            foreach (var brush in brushes)
            {
                if (brush.TryGetComponent<IInteractable>(out var interactor))
                {
                    interactor.OnInteractionEntered += OnInteractionEntered;
                    interactor.OnInteractionExited += OnInteractionExited;
                }
            }
        }

        private void OnDisable()
        {
            foreach (var brush in brushes)
            {
                if (brush.TryGetComponent<IInteractable>(out var interactor))
                {
                    interactor.OnInteractionEntered -= OnInteractionEntered;
                    interactor.OnInteractionExited -= OnInteractionExited;
                }
            }
        }

        private void Start()
        {
            gameplayCancellation = new CancellationTokenSource();
            StartGameAsync(gameplayCancellation.Token).Forget();
        }

        private void Update()
        {
            pedestal.AddSpinSpeed(crank.RotationDelta);
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
            gameplaySystem.ResetScoreEntries();
            gameplaySystem.ResetMultiplier();
            currentPaintableScore = 0;
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
            gameplaySystem.CurrentPaintableData = paintable;

            // Game loop
            do
            {
                // Switch painted object
                if (paintable.PaintAmount >= 1f)
                {
                    var scoreMultiplier = gameplaySystem.CurrentMultiplier;
                    var scoreResult = currentPaintableScore + (int)(paintable.Data.Score * scoreMultiplier);

                    gameplaySystem.RecordScore(
                        new PaintableScoreEntry(
                            data: paintable.Data,
                            maskTexture: paintable.MaskTexture,
                            paintableScore: currentPaintableScore,
                            baseScore: paintable.Data.Score,
                            scoreMultiplier: gameplaySystem.CurrentMultiplier,
                            scoreResult: scoreResult
                        )
                    );

                    // Slide out old object
                    gameplaySystem.State = GameplayState.SpawningObject;
                    gameplaySystem.CurrentPaintableData = null;

                    await paintable.SlideOutAsync(cancellationToken);
                    paintable.OnPainted -= OnObjectPainted;

                    // Slide in new object
                    paintable = CreatePaintable(pedestal.ObjectParent);
                    paintable.gameObject.SetActive(false);
                    paintable.OnPainted += OnObjectPainted;

                    gameplaySystem.PaintAmount = 0f;
                    currentPaintableScore = 0;
                    gameplaySystem.State = GameplayState.PaintingObject;

                    await paintable.SlideInAsync(cancellationToken);

                    gameplaySystem.CurrentPaintableData = paintable;
                }

                var targetMultiplier = data.GetScoreMultiplier(pedestal.SpinSpeed);
                gameplaySystem.CurrentMultiplier = Mathf.Lerp(
                    gameplaySystem.CurrentMultiplier,
                    targetMultiplier,
                    multiplierDecaySpeed * Time.deltaTime
                );

                await UniTask.Yield(cancellationToken);
            } while (gameplaySystem.State != GameplayState.GameOver);

            // Record last object sample
            {
                gameplaySystem.RecordScore(
                    new PaintableScoreEntry(
                        data: paintable.Data,
                        maskTexture: paintable.MaskTexture,
                        paintableScore: currentPaintableScore,
                        baseScore: 0,
                        scoreMultiplier: gameplaySystem.CurrentMultiplier,
                        scoreResult: currentPaintableScore
                    )
                );
            }

            LoadGameOverScene();
        }

        private void OnInteractionEntered(InteractableInteractionEnteredArgs args)
        {
            palette.MoveAside();
            // crank.MoveAside();
        }

        private void OnInteractionExited(InteractableInteractionExitedArgs args)
        {
            palette.MoveBack();
            // crank.MoveBack();
        }

        private void OnObjectPainted(PaintedArgs args)
        {
            if (gameplaySystem.State != GameplayState.PaintingObject)
            {
                return;
            }

            gameplaySystem.PaintAmount = args.PaintAmount;
            var scoreThisTick = Mathf.RoundToInt(args.PaintedScore * gameplaySystem.CurrentMultiplier);
            currentPaintableScore += scoreThisTick;
            gameplaySystem.Score += scoreThisTick;
        }

        private PaintableActor CreatePaintable(Transform parent)
        {
            if (paintableQueue.Count <= 0)
            {
                ReloadPaintableQueue();
            }

            var index = paintableQueue.Count - 1;
            var pedestalObject = paintableQueue[index];
            paintableQueue.RemoveAt(index);

            return pedestalObject.CreatePaintable(
                pos: parent.position,
                rot: Quaternion.identity,
                parent: parent
            );
        }

        private void ReloadPaintableQueue()
        {
            paintableQueue.Clear();
            paintableQueue.AddRange(data.Paintables);
            paintableQueue.Shuffle();
        }

        private void LoadGameOverScene()
        {
            sceneSystem.LoadScene(new SceneLoadArgs(data.GameOverScene));
        }

        private async UniTask PlayIntroAsync(CancellationToken cancellationToken)
        {
            if (data.IsPlayIntro && introPlayable)
            {
                introCamera.enabled = true;
                await introPlayable.PlayAsync(cancellationToken);
                introCamera.enabled = false;
            }
        }
    }
}
