namespace NMorph
{
    public class MorphGroup
    {
        private readonly AlterationStore _alterationStore = new AlterationStore();

        public Morph<T> Select<T>() where T : class
        {
            return new Morph<T>(_alterationStore);
        }
        
        public void Reset()
        {
            _alterationStore.Reset();
        }
    }
}