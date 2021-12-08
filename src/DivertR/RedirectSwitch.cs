using System.Threading;

namespace DivertR
{
    public class RedirectSwitch : IRedirectSwitch
    {
        private volatile int _isEnabled;

        public RedirectSwitch(bool isEnabled = true)
        {
            _isEnabled = isEnabled ? 1 : 0;
        }

        public bool IsEnabled => _isEnabled == 1;

        public void Enable()
        {
            Interlocked.Exchange(ref _isEnabled, 1);
        }

        public void Disable()
        {
            Interlocked.Exchange(ref _isEnabled, 0);
        }
    }
}