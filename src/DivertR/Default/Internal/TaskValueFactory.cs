using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DivertR.Default.Internal
{
    internal class TaskValueFactory
    {
        private readonly ConcurrentDictionary<Type, IInternalTaskFactory> _taskFactories = new ConcurrentDictionary<Type, IInternalTaskFactory>();
        private readonly ConcurrentDictionary<Type, IInternalValueTaskFactory> _valueTaskFactories = new ConcurrentDictionary<Type, IInternalValueTaskFactory>();

        public Task CreateTaskOf(Type taskType, IDefaultValueFactory defaultValueFactory)
        {
            var taskFactory = _taskFactories.GetOrAdd(taskType, type =>
            {
                var taskFactoryType = typeof(InternalTaskFactory<>).MakeGenericType(taskType.GenericTypeArguments);
                return (IInternalTaskFactory) Activator.CreateInstance(taskFactoryType);
            });

            return taskFactory.CreateTask(defaultValueFactory);
        }
        
        public object CreateValueTaskOf(Type taskType, IDefaultValueFactory defaultValueFactory)
        {
            var taskFactory = _valueTaskFactories.GetOrAdd(taskType, type =>
            {
                var taskFactoryType = typeof(InternalValueTaskFactory<>).MakeGenericType(taskType.GenericTypeArguments);
                return (IInternalValueTaskFactory) Activator.CreateInstance(taskFactoryType);
            });

            return taskFactory.CreateValueTask(defaultValueFactory);
        }

        private interface IInternalTaskFactory
        {
            Task CreateTask(IDefaultValueFactory defaultValueFactory);
        }

        private class InternalTaskFactory<TResult> : IInternalTaskFactory
        {
            public Task CreateTask(IDefaultValueFactory defaultValueFactory)
            {
                var value = (TResult) defaultValueFactory.Create(typeof(TResult))!;
                
                return Task.FromResult<TResult>(value);
            }
        }
        
        private interface IInternalValueTaskFactory
        {
            object CreateValueTask(IDefaultValueFactory defaultValueFactory);
        }

        private class InternalValueTaskFactory<TResult> : IInternalValueTaskFactory
        {
            public object CreateValueTask(IDefaultValueFactory defaultValueFactory)
            {
                var value = (TResult) defaultValueFactory.Create(typeof(TResult))!;

                return new ValueTask<TResult>(value);
            }
        }
    }
}