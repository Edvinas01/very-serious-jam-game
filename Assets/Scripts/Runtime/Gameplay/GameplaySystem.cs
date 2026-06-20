using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using InSun.GameCore;
using InSun.GameCore.Objects;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal sealed class GameplaySystem : MonoBehaviour, ILifecycleListener
    {
        [SerializeField]
        private Transform fullyPaintedObjectTransform;

        private readonly List<PedestalObjectActor> fullyPaintedObjects = new();

        private IObjectGroup<PlayerActor> players;
        private IObjectGroup<PedestalActor> pedestals;
        private IObjectGroup<PedestalObjectActor> pedestalObjects;

        private CancellationTokenSource gameplayCancellation;
        private GameplayController context;

        private GameplayState currentState = GameplayState.None;
        private float currentRemainingTime;
        private int currentScore;

        public IReadOnlyList<PedestalObjectActor> FullyPaintedObjects => fullyPaintedObjects;

        public float PaintAmount
        {
            get
            {
                if (TryGetPedestalObject(out var pedestalObject))
                {
                    return pedestalObject.PaintAmount;
                }

                return 0f;
            }
        }

        public float RemainingTime
        {
            get => currentRemainingTime;
            private set => currentRemainingTime = Mathf.Clamp(value, 0f, context.GameplayDuration);
        }

        public int Score
        {
            get => currentScore;
            private set
            {
                var valuePrev = currentScore;
                var valueNext = Mathf.Clamp(value, 0, context.MaxScore);

                if (valuePrev == valueNext)
                {
                    return;
                }

                Debug.Log($"Score changed {valuePrev}->{valueNext}", this);
                currentScore = value;

                Game.PublishMessage(new ScoreChangedMessage(valuePrev, valueNext));
            }
        }

        public GameplayState State
        {
            get => currentState;
            private set
            {
                var valuePrev = currentState;
                var valueNext = value;

                if (valuePrev == valueNext)
                {
                    return;
                }

                Debug.Log($"State changed {valuePrev}->{valueNext}", this);
                currentState = value;

                Game.PublishMessage(new GameplayStateChangedMessage(valuePrev, valueNext));
            }
        }

        public void OnInitialized()
        {
            players = Game.GetObjectGroup<PlayerActor>();
            players.OnObjectAdded += OnPlayerAdded;
            players.OnObjectRemoved += OnPlayerRemoved;

            pedestals = Game.GetObjectGroup<PedestalActor>();
            pedestals.OnObjectAdded += OnPedestalAdded;
            pedestals.OnObjectRemoved += OnPedestalRemoved;

            pedestalObjects = Game.GetObjectGroup<PedestalObjectActor>();
            pedestalObjects.OnObjectAdded += OnPedestalObjectAdded;
            pedestalObjects.OnObjectRemoved += OnPedestalObjectRemoved;
        }

        public void OnDisposed()
        {
            gameplayCancellation?.Cancel();
            gameplayCancellation?.Dispose();
            gameplayCancellation = null;
        }

        public void StartGame(GameplayController newController)
        {
            if (context != null)
            {
                Debug.LogWarning("Game is already started", this);
                return;
            }

            context = newController;

            gameplayCancellation?.Cancel();
            gameplayCancellation?.Dispose();
            gameplayCancellation = new CancellationTokenSource();

            StartGameAsync(gameplayCancellation.Token).Forget();
        }

        public void StopGame()
        {
            if (context == null)
            {
                Debug.LogWarning("Game is not started", this);
                return;
            }

            gameplayCancellation?.Cancel();
            gameplayCancellation?.Dispose();
            gameplayCancellation = null;

            context = null;
        }

        private async UniTaskVoid StartGameAsync(CancellationToken cancellationToken)
        {
            if (TryGetPlayer(out var player) == false)
            {
                Debug.LogError("Cannot start game, no players found", this);
                return;
            }

            if (TryGetPedestal(out var pedestal) == false)
            {
                Debug.LogError("Cannot start game, no pedestals found", this);
                return;
            }

            State = GameplayState.Introduction;

            // Init game
            foreach (var paintedObject in fullyPaintedObjects)
            {
                Destroy(paintedObject.gameObject);
            }

            fullyPaintedObjects.Clear();
            RemainingTime = context.GameplayDuration;
            Score = 0;

            // // disable player so no movement during intro
            // player.DisableInteraction();
            // player.DisableCamera();
            //
            // // intro anim
            // await context.PlayIntroAsync(cancellationToken);

            //
            // Enable player
            player.EnableInteraction();
            player.EnableCamera();

            // Spaw pedestal object
            State = GameplayState.SpawningObject;
            var pedestalObject = context.CreatePedestalObject(pedestal.ObjectParent);
            pedestalObject.OnPainted += OnObjectPainted;
            await pedestalObject.SlideInAsync(cancellationToken);
            State = GameplayState.PaintingObject;

            // Game loop
            do
            {
                RemainingTime -= Time.deltaTime;

                // GG: out of time
                if (RemainingTime <= 0f)
                {
                    State = GameplayState.GameOver;
                    break;
                }

                // GG: reached max score
                if (Score > context.MaxScore)
                {
                    State = GameplayState.GameOver;
                    break;
                }

                // Switch painted object
                if (pedestalObject.PaintAmount >= 1f)
                {
                    Score += pedestalObject.Data.Score;

                    // Slide out old object
                    State = GameplayState.SpawningObject;
                    await pedestalObject.SlideOutAsync(cancellationToken);
                    fullyPaintedObjects.Add(pedestalObject);

                    pedestalObject.transform.position = fullyPaintedObjectTransform.transform.position;
                    pedestalObject.transform.parent = fullyPaintedObjectTransform;
                    pedestalObject.gameObject.SetActive(false);
                    pedestalObject.OnPainted -= OnObjectPainted;

                    // Slide in new object
                    pedestalObject = context.CreatePedestalObject(pedestal.ObjectParent);
                    pedestalObject.OnPainted += OnObjectPainted;
                    await pedestalObject.SlideInAsync(cancellationToken);
                    State = GameplayState.PaintingObject;
                }

                await UniTask.Yield(cancellationToken);
            } while (State != GameplayState.GameOver);

            context.LoadGameOverScene();
            State = GameplayState.GameOver;
        }

        private bool TryGetPlayer(out PlayerActor player)
        {
            player = players.FirstOrDefault();
            return player;
        }

        private bool TryGetPedestal(out PedestalActor pedestal)
        {
            pedestal = pedestals.FirstOrDefault();
            return pedestal;
        }

        private bool TryGetPedestalObject(out PedestalObjectActor pedestalObject)
        {
            pedestalObject = pedestalObjects.FirstOrDefault();
            return pedestalObject;
        }

        private void OnObjectPainted(float paintAmount)
        {
            Game.PublishMessage(new PaintAmountChangedMessage(paintAmount));
        }

        private void OnPedestalAdded(PedestalActor pedestal)
        {
            Debug.Log($"Added Pedestal '{pedestal.name}'", pedestal);
        }

        private void OnPedestalRemoved(PedestalActor pedestal)
        {
            Debug.Log($"Removed Pedestal '{pedestal.name}'", pedestal);
        }

        private void OnPedestalObjectAdded(PedestalObjectActor pedestalObject)
        {
            Debug.Log($"Added PedestalObject '{pedestalObject.name}'", pedestalObject);
        }

        private void OnPedestalObjectRemoved(PedestalObjectActor pedestalObject)
        {
            Debug.Log($"Removed PedestalObject '{pedestalObject.name}'", pedestalObject);
        }

        private void OnPlayerAdded(PlayerActor player)
        {
            Debug.Log($"Added Player '{player.name}'", player);
        }

        private void OnPlayerRemoved(PlayerActor player)
        {
            Debug.Log($"Removed Player '{player.name}'", player);
        }
    }
}
