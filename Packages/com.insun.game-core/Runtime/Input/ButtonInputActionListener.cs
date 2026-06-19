using UnityEngine.InputSystem;

namespace InSun.GameCore.Input
{
    public class ButtonInputActionListener : InputActionListener<bool>
    {
        protected override bool ReadValue(InputAction.CallbackContext ctx)
        {
            return ctx.ReadValueAsButton();
        }

        protected override bool ReadValue()
        {
            return InputAction.phase == InputActionPhase.Performed;
        }
    }
}
