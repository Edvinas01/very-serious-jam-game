namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal readonly struct PaintedArgs
    {
        public float PaintAmount { get; }

        public int PaintedScore { get; }

        public PaintedArgs(float paintAmount, int paintedScore)
        {
            PaintAmount = paintAmount;
            PaintedScore = paintedScore;
        }
    }
}
