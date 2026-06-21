using System.Collections.Generic;
using InSun.GameCore;
using InSun.GameCore.Objects;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal sealed class GameplaySystem : MonoBehaviour, IUpdateListener
    {
        private readonly List<PaintableScoreEntry> scoreEntries = new();

        private GameplayState currentState = GameplayState.None;
        private float currentRemainingTime;
        private float currentPaintAmount;
        private int currentScore;
        private string currentPaintableName;

        public float CurrentMultiplier { get; set; } = 1f;

        public string CurrentPaintableName
        {
            get => currentPaintableName;
            set
            {
                currentPaintableName = value;
                Game.PublishMessage(new PaintableNameChangedMessage(value));
            }
        }

        public IReadOnlyList<PaintableScoreEntry> ScoreEntries => scoreEntries;

        public float PaintAmount
        {
            get => currentPaintAmount;
            set
            {
                var valuePrev = currentPaintAmount;
                var valueNext = value;

                if (Mathf.Approximately(valuePrev, valueNext))
                {
                    return;
                }

                currentPaintAmount = valueNext;

                Game.PublishMessage(new PaintAmountChangedMessage(valueNext));
            }
        }

        public float RemainingTime
        {
            get => currentRemainingTime;
            set
            {
                currentRemainingTime = Mathf.Max(0f, value);

                var remainingSecondsPrev = Mathf.CeilToInt(currentRemainingTime);
                var remainingSecondsNext = Mathf.CeilToInt(value);

                if (remainingSecondsPrev != remainingSecondsNext)
                {
                    Game.PublishMessage(new RemainingTimeChangedMessage(currentRemainingTime));
                }
            }
        }

        public int Score
        {
            get => currentScore;
            set
            {
                var valuePrev = currentScore;
                var valueNext = value;

                if (valuePrev == valueNext)
                {
                    return;
                }

                Debug.Log($"Score changed {valuePrev}->{valueNext}", this);
                currentScore = valueNext;

                Game.PublishMessage(new ScoreChangedMessage(valuePrev, valueNext));
            }
        }

        public GameplayState State
        {
            get => currentState;
            set
            {
                var valuePrev = currentState;
                var valueNext = value;

                if (valuePrev == valueNext)
                {
                    return;
                }

                Debug.Log($"State changed {valuePrev}->{valueNext}", this);
                currentState = valueNext;

                Game.PublishMessage(new GameplayStateChangedMessage(valuePrev, valueNext));
            }
        }

        public void OnUpdated(float deltaTime)
        {
            if (currentState is GameplayState.None or GameplayState.GameOver or GameplayState.Introduction)
            {
                return;
            }

            RemainingTime -= deltaTime;

            if (currentRemainingTime <= 0f)
            {
                State = GameplayState.GameOver;
            }
        }

        public void ResetMultiplier()
        {
            CurrentMultiplier = 1f;
        }

        public void ResetScoreEntries()
        {
            foreach (var scoreEntry in scoreEntries)
            {
                if (scoreEntry.MaskTexture)
                {
                    Destroy(scoreEntry.MaskTexture);
                }
            }

            scoreEntries.Clear();
        }

        public void RecordScore(PaintableScoreEntry entry)
        {
            Score += (int)(entry.BaseScore * entry.ScoreMultiplier);
            scoreEntries.Add(entry);
        }
    }
}
