using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DivertR.Default.Internal
{
    internal class DefaultAwaitableFactory
    {
        private readonly ConcurrentDictionary<Type, IInternalTaskFactory> _taskFactories = new ConcurrentDictionary<Type, IInternalTaskFactory>();
        private readonly ConcurrentDictionary<Type, IInternalValueTaskFactory> _valueTaskFactories = new ConcurrentDictionary<Type, IInternalValueTaskFactory>();
        
        private readonly IDefaultValueFactory _defaultValueFactory;

        public DefaultAwaitableFactory(IDefaultValueFactory defaultValueFactory)
        {
            _defaultValueFactory = defaultValueFactory;
        }

        public Task CreateTaskOf(Type taskType)
        {
            var taskFactory = _taskFactories.GetOrAdd(taskType, type =>
            {
                var taskFactoryType = typeof(InternalTaskFactory<>).MakeGenericType(taskType.GenericTypeArguments);
                return (IInternalTaskFactory) Activator.CreateInstance(taskFactoryType, _defaultValueFactory);
            });

            return taskFactory.CreateTask();
        }
        
        public object CreateValueTaskOf(Type taskType)
        {
            var taskFactory = _valueTaskFactories.GetOrAdd(taskType, type =>
            {
                var taskFactoryType = typeof(InternalValueTaskFactory<>).MakeGenericType(taskType.GenericTypeArguments);
                return (IInternalValueTaskFactory) Activator.CreateInstance(taskFactoryType, _defaultValueFactory);
            });

            return taskFactory.CreateValueTask();
        }

        private interface IInternalTaskFactory
        {
            Task CreateTask();
        }

        private class InternalTaskFactory<TResult> : IInternalTaskFactory
        {
            private readonly IDefaultValueFactory _defaultValueFactory;

            public InternalTaskFactory(IDefaultValueFactory defaultValueFactory)
            {
                _defaultValueFactory = defaultValueFactory;
            }
            
            public Task CreateTask()
            {
                var value = (TResult) _defaultValueFactory.GetDefaultValue(typeof(TResult))!;
                
                return Task.FromResult<TResult>(value);
            }
        }
        
        private interface IInternalValueTaskFactory
        {
            object CreateValueTask();
        }

        private class InternalValueTaskFactory<TResult> : IInternalValueTaskFactory
        {
            private readonly IDefaultValueFactory _defaultValueFactory;

            public InternalValueTaskFactory(IDefaultValueFactory defaultValueFactory)
            {
                _defaultValueFactory = defaultValueFactory;
            }
            
            public object CreateValueTask()
            {
                var value = (TResult) _defaultValueFactory.GetDefaultValue(typeof(TResult))!;

                return new ValueTask<TResult>(value);
            }
        }
    }
}