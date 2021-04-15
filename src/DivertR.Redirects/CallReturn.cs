namespace DivertR.Redirects
{
    internal class CallReturn
    {
        public bool IsSet { get; private set; }

        private object? _returnedObject;
        
        public object? ReturnedObject
        {
            get => _returnedObject;
            set
            {
                IsSet = true;
                _returnedObject = value;
            }
        }
    }
}