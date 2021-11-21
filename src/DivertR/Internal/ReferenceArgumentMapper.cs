using System;

namespace DivertR.Internal
{
    internal class ReferenceArgumentMapper
    {
        private readonly (IReferenceArgumentFactory ArgFactory, int Index)[] _refArgumentIndex;

        public ReferenceArgumentMapper((IReferenceArgumentFactory ArgFactory, int Index)[] refArgumentIndex)
        {
            _refArgumentIndex = refArgumentIndex;
        }

        public object[] MapToReferences(object[] args)
        {
            var mappedArgs = new object[args.Length];
            Array.Copy(args, mappedArgs, args.Length);

            foreach (var refIndex in _refArgumentIndex)
            {
                mappedArgs[refIndex.Index] = refIndex.ArgFactory.Create(args[refIndex.Index]);
            }

            return mappedArgs;
        }

        public void WriteReferences(object[] mappedArgs, object[] args)
        {
            foreach (var refIndex in _refArgumentIndex)
            {
                args[refIndex.Index] = refIndex.ArgFactory.GetRefValue(mappedArgs[refIndex.Index]);
            }
        }
    }
}