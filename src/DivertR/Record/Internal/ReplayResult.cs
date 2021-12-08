namespace DivertR.Record.Internal
{
    internal class ReplayResult : IReplayResult
    {
        public ReplayResult(int count)
        {
            Count = count;
        }

        public int Count { get; }
    }
}