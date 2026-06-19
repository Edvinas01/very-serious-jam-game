namespace InSun.GameCore.Objects
{
    public interface ILifecycleListener
    {
        public void OnInitialized();

        public void OnDisposed();
    }
}
