using System.Collections.Generic;
using InSun.GameCore;
using InSun.GameCore.Objects;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal sealed class GameplaySystem : MonoBehaviour, IUpdateListener
    {
        [SerializeField]
        private Transform fullyPaintedObjectTransform;

        private readonly List<PaintableActor> fullyPaintedObjects = new();

        private GameplayState currentState = GameplayState.None;
        private float currentRemainingTime;
        private float currentPaintAmount;
        private int currentScore;

        public IReadOnlyList<PaintableActor> FullyPaintedObjects => fullyPaintedObjects;

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
            if (currentState is GameplayState.None or GameplayState.GameOver)
            {
                return;
            }

            RemainingTime -= deltaTime;

            if (currentRemainingTime <= 0f)
            {
                State = GameplayState.GameOver;
            }
        }

        public void ClearPaintedObjects()
        {
            foreach (var paintedObject in fullyPaintedObjects)
            {
                if (paintedObject)
                {
                    Destroy(paintedObject.gameObject);
                }
            }

            fullyPaintedObjects.Clear();
        }

        public void Store(PaintableActor paintable)
        {
            fullyPaintedObjects.Add(paintable);

            paintable.transform.position = fullyPaintedObjectTransform.position;
            paintable.transform.parent = fullyPaintedObjectTransform;
            paintable.gameObject.SetActive(false);
        }
    }
}
