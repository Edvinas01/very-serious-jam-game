using InSun.GameCore.Messaging;

namespace DoubleD.VerySeriousJamGame.Runtime.Pausing
{
    internal sealed class PauseStateChangedMessage : IMessage
    {
        public bool IsPausedPrev { get; }

        public bool IsPausedNext { get; }

        public PauseStateChangedMessage(bool isPausedPrev, bool isPausedNext)
        {
            IsPausedPrev = isPausedPrev;
            IsPausedNext = isPausedNext;
        }
    }
}
