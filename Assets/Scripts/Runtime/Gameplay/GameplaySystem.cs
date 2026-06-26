using System.Collections.Generic;
using InSun.GameCore;
using InSun.GameCore.Objects;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal sealed class GameplaySystem : MonoBehaviour, IUpdateListener
    {
        private readonly List<PaintableScoreEntry> scoreEntries = new();
        private readonly List<float> paintableColorScores = new();

        private GameplayState currentState = GameplayState.None;
        private PaintableActor currentPaintable;
        private float currentRemainingTime;
        private float currentPaintAmount;
        private int currentScore;

        public float CurrentMultiplier { get; set; } = 1f;

        public float CreativityMultiplier { get; private set; } = 1f;

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
                var remainingSecondsPrev = Mathf.CeilToInt(currentRemainingTime);
                currentRemainingTime = Mathf.Max(0f, value);
                var remainingSecondsNext = Mathf.CeilToInt(currentRemainingTime);

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

        public PaintableActor CurrentPaintableData
        {
            set
            {
                var valuePrev = currentPaintable;
                var valueNext = value;

                if (valuePrev == valueNext)
                {
                    return;
                }

                var namePrev = valuePrev ? valuePrev.name : "N/A";
                var nameNext = valueNext ? valueNext.name : "N/A";
                Debug.Log($"Paintable changed {namePrev}->{nameNext}", this);

                currentPaintable = valueNext;

                Game.PublishMessage(new CurrentPaintableChangedMessage(valueNext));
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

        public void ResetScoreMultiplier()
        {
            CurrentMultiplier = 1f;
        }

        public void ResetCreativityMultiplier()
        {
            paintableColorScores.Clear();
            CreativityMultiplier = 1f;
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

        public bool TryGetPaintable(out PaintableActor paintable)
        {
            paintable = currentPaintable;
            return paintable;
        }


        public void RecordColors(IReadOnlyDictionary<Color, int> counts)
        {
            if (counts.Count == 0)
            {
                return;
            }

            var min = int.MaxValue;
            var max = int.MinValue;
            foreach (var count in counts.Values)
            {
                if (count < min)
                {
                    min = count;
                }

                if (count > max)
                {
                    max = count;
                }
            }

            var balance = counts.Count > 1 ? (float)min / max : 0f;
            paintableColorScores.Add(balance);
        }

        public void UpdateCreativity(Vector2 range)
        {
            if (paintableColorScores.Count == 0)
            {
                CreativityMultiplier = range.x;
                return;
            }

            var avg = 0f;
            foreach (var score in paintableColorScores)
            {
                avg += score;
            }

            var average = avg / paintableColorScores.Count;
            CreativityMultiplier = Mathf.Lerp(range.x, range.y, average);
        }
    }
}
