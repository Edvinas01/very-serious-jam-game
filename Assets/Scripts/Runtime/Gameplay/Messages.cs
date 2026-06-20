using InSun.GameCore.Messaging;

namespace DoubleD.VerySeriousJamGame.Runtime.Gameplay
{
    internal readonly struct GameplayStateChangedMessage : IMessage
    {
        public GameplayState StatePrev { get; }

        public GameplayState StateNext { get; }

        public GameplayStateChangedMessage(GameplayState statePrev, GameplayState stateNext)
        {
            StatePrev = statePrev;
            StateNext = stateNext;
        }
    }

    internal readonly struct ScoreChangedMessage : IMessage
    {
        public int ScorePrev { get; }

        public int ScoreNext { get; }

        public ScoreChangedMessage(int scorePrev, int scoreNext)
        {
            ScorePrev = scorePrev;
            ScoreNext = scoreNext;
        }
    }

    internal readonly struct PaintAmountChangedMessage : IMessage
    {
        public float PaintAmount { get; }

        public PaintAmountChangedMessage(float paintAmount)
        {
            PaintAmount = paintAmount;
        }
    }
}
