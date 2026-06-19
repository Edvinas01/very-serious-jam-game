using UnityEngine;

namespace InSun.GameCore.UI
{
    public enum ViewState
    {
        [Tooltip("View is the process of hiding")]
        Hiding = 0,

        [Tooltip("View has finished hiding and is no longer visible")]
        Hidden = 1,

        [Tooltip("View is the process of showing")]
        Showing = 2,

        [Tooltip("View has finished showing and is now fully visible")]
        Shown = 3,
    }
}
