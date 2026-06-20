using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using InSun.GameCore;
using InSun.GameCore.Objects;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal sealed class GameplaySystem : ILifecycleListener
    {
        private IObjectGroup<PlayerActor> players;
        private IObjectGroup<PedestalActor> pedestals;
        private IObjectGroup<PedestalObjectActor> pedestalObjects;

        private CancellationTokenSource gameplayCancellation;
        private GameplayController context;

        public void OnInitialized()
        {
            players = Game.GetObjectGroup<PlayerActor>();
            players.OnObjectAdded += OnPlayerAdded;
            players.OnObjectAdded += OnPlayerRemoved;

            pedestals = Game.GetObjectGroup<PedestalActor>();
            pedestals.OnObjectAdded += OnPedestalAdded;
            pedestals.OnObjectAdded += OnPedestalRemoved;

            pedestalObjects = Game.GetObjectGroup<PedestalObjectActor>();
            pedestalObjects.OnObjectAdded += OnPedestalObjectAdded;
            pedestalObjects.OnObjectAdded += OnPedestalObjectRemoved;
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
                Debug.LogWarning("Game is already started");
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
                Debug.LogWarning("Game is not started");
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
                Debug.LogError("Cannot start game, no players found");
                return;
            }

            if (TryGetPedestal(out var pedestal) == false)
            {
                Debug.LogError("Cannot start game, no pedestals found");
                return;
            }

            // disable player so no movement during intro
            player.DisableInteraction();
            player.DisableCamera();

            // intro anim
            await context.PlayIntroAsync(cancellationToken);

            // enable player
            player.EnableInteraction();
            player.EnableCamera();

            // spawn pedestal object
            var pedestalObject = context.CreatePedestalObject(pedestal.ObjectParent);

            // wait for GG
            PlayerState playerState;
            do
            {
                playerState = GetPlayerState(player);
                await UniTask.Yield(cancellationToken);
            } while (playerState == PlayerState.Playing);

            // check result
            switch (playerState)
            {
                case PlayerState.Won:
                {
                    context.LoadVictoryScene();
                    break;
                }
                case PlayerState.Lost:
                {
                    context.LoadGameOverScene();
                    break;
                }
                default:
                {
                    Debug.LogError($"Unsupported player state {playerState}");
                    break;
                }
            }
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

        private static PlayerState GetPlayerState(PlayerActor player)
        {
            if (player.Score >= player.MaxScore)
            {
                return PlayerState.Won;
            }

            if (player.Score <= 0)
            {
                return PlayerState.Lost;
            }

            return PlayerState.Playing;
        }

        private enum PlayerState
        {
            Playing,
            Won,
            Lost,
        }

        // Logging
        private void OnPedestalAdded(PedestalActor pedestal)
        {
            Debug.Log($"Added Pedestal {pedestal.name}", pedestal);
        }

        private void OnPedestalRemoved(PedestalActor pedestal)
        {
            Debug.Log($"Removed Pedestal {pedestal.name}", pedestal);
        }

        private void OnPedestalObjectAdded(PedestalObjectActor pedestalObject)
        {
            Debug.Log($"Added PedestalObject {pedestalObject.name}", pedestalObject);
        }

        private void OnPedestalObjectRemoved(PedestalObjectActor pedestalObject)
        {
            Debug.Log($"Removed PedestalObject {pedestalObject.name}", pedestalObject);
        }

        private void OnPlayerAdded(PlayerActor player)
        {
            Debug.Log($"Added Player {player.name}", player);
        }

        private void OnPlayerRemoved(PlayerActor player)
        {
            Debug.Log($"Removed Player {player.name}", player);
        }
    }
}
