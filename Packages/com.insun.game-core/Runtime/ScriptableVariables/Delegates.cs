namespace InSun.GameCore.ScriptableVariables
{
    public delegate void ValueChangedDelegate<in T>(T prevValue, T nextValue);

    public delegate void ValueChangedDelegate();
}
