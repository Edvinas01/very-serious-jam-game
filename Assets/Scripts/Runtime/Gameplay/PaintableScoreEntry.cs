using System;
using UnityEngine;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal readonly struct PaintableScoreEntry
    {
        public PaintableData Data { get; }

        public Texture2D MaskTexture { get; }

        public int PaintableScore { get; }

        public float TotalScoreMultiplier { get; }

        public int TotalScore { get; }

        public PaintableScoreEntry(
            PaintableData data,
            Texture2D maskTexture,
            int paintableScore,
            float totalScoreMultiplier,
            int totalScore
        )
        {
            Data = data;
            MaskTexture = maskTexture;
            PaintableScore = paintableScore;
            TotalScoreMultiplier = totalScoreMultiplier;
            TotalScore = totalScore;
        }
    }
}
