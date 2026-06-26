using System.Collections.Generic;
using InSun.GameCore.Scenes;
using InSun.GameCore.SunnyInspector;
using UnityEngine;
using UnityEngine.Playables;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    [SunnySettings(MenuPath = "Gameplay")]
    [CreateAssetMenu(
        menuName = "DoubleD/Very Serious Jam Game/Gameplay Data",
        fileName = "Data_Gameplay"
    )]
    internal sealed class GameplayData : ScriptableObject
    {
        [Header("Gameplay: Intro / Outro")]
        [SerializeField]
        private bool isPlayIntro = true;

        [ShowIf(nameof(isPlayIntro))]
        [SerializeField]
        private PlayableAsset introPlayable;

        [SerializeField]
        private bool isPlayOutro = true;

        [ShowIf(nameof(isPlayOutro))]
        [SerializeField]
        private PlayableAsset outroPlayable;

        [Header("Gameplay: Time & Score")]
        [SerializeField]
        private float gameplayDuration = 60f;

        [Min(0)]
        [SerializeField]
        private int startingScore;

        [SerializeField]
        private Vector2 creativityMultiplierRange = new(1f, 10f);

        [Tooltip("x - spin speed, y - score multiplier")]
        [SerializeField]
        private AnimationCurve scoreMultiplierCurve;

        [Header("Scenes")]
        [SerializeField]
        private SceneData gameOverScene;

        [Header("Pedestal Objects")]
        [SerializeField]
        private PaintableData firstPaintable;

        [SerializeField]
        private List<PaintableData> paintables;

        public bool IsPlayIntro => isPlayIntro;

        public PlayableAsset IntroPlayable => introPlayable;

        public bool IsPlayOutro => isPlayOutro;

        public PlayableAsset OutroPlayable => outroPlayable;

        public float GameplayDuration => gameplayDuration;

        public int StartingScore => startingScore;

        public Vector2 CreativityMultiplierRange => creativityMultiplierRange;

        public SceneData GameOverScene => gameOverScene;

        public PaintableData FirstPaintable => firstPaintable;

        public IReadOnlyList<PaintableData> Paintables => paintables;

        public float GetScoreMultiplier(float speed)
        {
            return scoreMultiplierCurve.Evaluate(Mathf.Abs(speed));
        }
    }
}
