using System.Collections.Concurrent;

namespace DivertR
{
    public class RedirectSwitch : IRedirectSwitch
    {
        private readonly ConcurrentStack<bool> _switchState = new ConcurrentStack<bool>();

        public RedirectSwitch(bool isEnabled = true)
        {
            _switchState.Push(isEnabled);
        }

        public bool IsEnabled
        {
            get
            {
                _switchState.TryPeek(out var result);

                return result;
            }
        }

        public void Enable()
        {
            _switchState.Push(true);
        }

        public void Disable()
        {
            _switchState.Push(false);
        }
    }
}