using System.Reflection;
using UnityEngine.Events;

namespace InSun.GameCore.Utilities
{
    public static class UnityEventUtilities
    {
        private static readonly MethodInfo RebuildPersistentCallsMethod = typeof(UnityEventBase).GetMethod(
            "RebuildPersistentCallsIfNeeded",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        public static void RebuildPersistentCallsIfNeeded(this UnityEventBase unityEvent)
        {
            RebuildPersistentCallsMethod?.Invoke(unityEvent, null);
        }
    }
}
