using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DivertR.Dummy.Internal
{
    internal class TaskValueFactory
    {
        private readonly ConcurrentDictionary<Type, IInternalTaskFactory> _taskFactories = new ConcurrentDictionary<Type, IInternalTaskFactory>();
        private readonly ConcurrentDictionary<Type, IInternalValueTaskFactory> _valueTaskFactories = new ConcurrentDictionary<Type, IInternalValueTaskFactory>();

        public Task CreateTaskOf(Type taskType, DummyValueFactory dummyValueFactory)
        {
            var taskFactory = _taskFactories.GetOrAdd(taskType, type =>
            {
                var taskFactoryType = typeof(InternalTaskFactory<>).MakeGenericType(taskType.GenericTypeArguments);
                return (IInternalTaskFactory) Activator.CreateInstance(taskFactoryType);
            });

            return taskFactory.CreateTask(dummyValueFactory);
        }
        
        public object CreateValueTaskOf(Type taskType, DummyValueFactory dummyValueFactory)
        {
            var taskFactory = _valueTaskFactories.GetOrAdd(taskType, type =>
            {
                var taskFactoryType = typeof(InternalValueTaskFactory<>).MakeGenericType(taskType.GenericTypeArguments);
                return (IInternalValueTaskFactory) Activator.CreateInstance(taskFactoryType);
            });

            return taskFactory.CreateValueTask(dummyValueFactory);
        }

        private interface IInternalTaskFactory
        {
            Task CreateTask(DummyValueFactory dummyValueFactory);
        }

        private class InternalTaskFactory<TResult> : IInternalTaskFactory
        {
            public Task CreateTask(DummyValueFactory dummyValueFactory)
            {
                var value = (TResult) dummyValueFactory.Create(typeof(TResult))!;
                
                return Task.FromResult<TResult>(value);
            }
        }
        
        private interface IInternalValueTaskFactory
        {
            object CreateValueTask(DummyValueFactory dummyValueFactory);
        }

        private class InternalValueTaskFactory<TResult> : IInternalValueTaskFactory
        {
            public object CreateValueTask(DummyValueFactory dummyValueFactory)
            {
                var value = (TResult) dummyValueFactory.Create(typeof(TResult))!;

                return new ValueTask<TResult>(value);
            }
        }
    }
}