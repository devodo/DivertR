namespace DivertR
{
    public interface IRedirectSwitch
    {
        public bool IsEnabled { get; }
        void Enable();
        void Disable();
    }
}