using System.Collections.Generic;
using InSun.GameCore.SunnyInspector;
using UnityEngine;

namespace InSun.GameCore.ScriptableVariables
{
    public abstract class ScriptableVariable<T> : ScriptableVariable
    {
        private bool isInitialized;
        private T currentValue;

        public virtual T BaseValue { get; }

        public T Value
        {
            get
            {
                if (isInitialized == false)
                {
                    currentValue = BaseValue;
                    isInitialized = true;
                }

                return currentValue;
            }
            set
            {
                isInitialized = true;

                var prevValue = currentValue;
                var nextValue = value;

                if (Equals(prevValue, nextValue))
                {
                    return;
                }

                currentValue = value;

                OnValueChanged?.Invoke(prevValue, nextValue);
            }
        }

        public event ValueChangedDelegate<T> OnValueChanged;

        protected void OnEnable()
        {
            if (isInitialized)
            {
                return;
            }

            currentValue = BaseValue;
            isInitialized = true;
        }

        protected void OnDisable()
        {
            currentValue = default;
            isInitialized = false;
            OnValueChanged = null;
        }

        protected virtual bool Equals(T prevValue, T newValue)
        {
            return EqualityComparer<T>.Default.Equals(prevValue, newValue);
        }

        protected override void OnReset()
        {
            currentValue = default;
            isInitialized = false;
            OnValueChanged = null;
        }

#if UNITY_EDITOR

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("Debug")]
        [Sirenix.OdinInspector.Button]
#endif
        private void TriggerEvents()
        {
            OnValueChanged?.Invoke(default, currentValue);
        }

#endif
    }

    [SunnySettings(MenuPath = "Scriptable Variables")]
    public abstract class ScriptableVariable : ScriptableObject
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.FoldoutGroup("General", Expanded = true)]
        [Sirenix.OdinInspector.MultiLineProperty(lines: 3)]
#endif
        [SerializeField]
        private string description;

        protected virtual void OnReset()
        {
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            var variables = Resources.FindObjectsOfTypeAll<ScriptableVariable>();
            foreach (var variable in variables)
            {
                variable.OnReset();
            }
        }
    }
}
