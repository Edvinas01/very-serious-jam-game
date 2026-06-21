using System.Collections.Generic;
using InSun.GameCore.Scenes;
using InSun.GameCore.SunnyInspector;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    [SunnySettings(MenuPath = "Gameplay")]
    [CreateAssetMenu(
        menuName = "DoubleD/Very Serious Jam Game/Gameplay Data",
        fileName = "Data_Gameplay"
    )]
    internal sealed class GameplayData : ScriptableObject
    {
        [Header("Gameplay")]
        [SerializeField]
        private float gameplayDuration = 60f;

        [Min(0)]
        [SerializeField]
        private int maxScore = 1000;

        [Min(0)]
        [SerializeField]
        private int startingScore;

        [Tooltip("x - spin speed, y - score multiplier")]
        [SerializeField]
        private AnimationCurve scoreMultiplierCurve;

        [Header("Scenes")]
        [SerializeField]
        private SceneData gameOverScene;

        [Header("Pedestal Objects")]
        [SerializeField]
        private List<PaintableData> paintables;

        public float GameplayDuration => gameplayDuration;

        public int MaxScore => maxScore;

        public int StartingScore => startingScore;

        public SceneData GameOverScene => gameOverScene;

        public IReadOnlyList<PaintableData> Paintables => paintables;

        public float GetScoreMultiplier(float speed)
        {
            return scoreMultiplierCurve.Evaluate(Mathf.Abs(speed));
        }
    }
}
