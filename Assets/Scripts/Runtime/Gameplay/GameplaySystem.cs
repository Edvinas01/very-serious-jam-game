using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DoubleD.VerySeriousJamGame.Runtime.Player;
using InSun.GameCore.Objects;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal sealed class GameplaySystem : ILifecycleListener
    {
        private readonly List<PlayerActor> players = new();

        private CancellationTokenSource gameplayCancellation;
        private GameplayController context;

        public void OnInitialized()
        {
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

        public void AddPlayer(PlayerActor player)
        {
            players.Add(player);
        }

        public void RemovePlayer(PlayerActor player)
        {
            players.Remove(player);
        }

        private async UniTaskVoid StartGameAsync(CancellationToken cancellationToken)
        {
            if (TryGetPlayer(out var player) == false)
            {
                Debug.LogError("Cannot start game, no players found");
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
    }
}
