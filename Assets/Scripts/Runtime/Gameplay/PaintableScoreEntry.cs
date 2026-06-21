using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal readonly struct PaintableScoreEntry
    {
        public PaintableData Data { get; }

        public Texture2D MaskTexture { get; }

        public float ScoreMultiplier { get; }

        public int ScoreResult { get; }

        public int PaintableScore { get; }

        public int BaseScore { get; }

        public PaintableScoreEntry(
            PaintableData data,
            Texture2D maskTexture,
            float scoreMultiplier,
            int scoreResult,
            int paintableScore,
            int baseScore
        )
        {
            Data = data;
            MaskTexture = maskTexture;
            ScoreMultiplier = scoreMultiplier;
            ScoreResult = scoreResult;
            PaintableScore = paintableScore;
            BaseScore = baseScore;
        }
    }
}
