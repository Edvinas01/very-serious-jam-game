using InSun.GameCore.Messaging;

namespace InSun.GameCore.Input
{
    internal readonly struct ControlSchemeChangedMessage : IMessage
    {
        public InputSystem.ControlSchemeType ControlScheme { get; }

        public ControlSchemeChangedMessage(InputSystem.ControlSchemeType controlScheme)
        {
            ControlScheme = controlScheme;
        }
    }
}
