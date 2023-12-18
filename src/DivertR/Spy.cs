using System;
using System.Diagnostics.CodeAnalysis;
using DivertR.Internal;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR
{
    /// <inheritdoc cref="ISpy{TTarget}" />
    public class Spy<TTarget> : Redirect<TTarget>, ISpy<TTarget> where TTarget : class?
    {
        private RecordStream<TTarget> _calls = null!;
        private readonly object _callsLock = new();

        public Spy(DiverterSettings? diverterSettings = null) : this(diverterSettings, null, true)
        {
        }
        
        public Spy(TTarget? root, DiverterSettings? diverterSettings = null)
            : this(diverterSettings, root, false)
        {
        }
        
        private Spy(DiverterSettings? diverterSettings, TTarget? root, bool hasRoot)
            : base(diverterSettings)
        {
            Mock = hasRoot ? Proxy() : Proxy(root);
            ResetAndConfigureRecord();
        }
        
        /// <inheritdoc />
        [NotNull]
        public TTarget Mock { get; }
        
        IRecordStream ISpy.Calls => CallsLocked;
        
        object ISpy.Mock => Mock;
        
        /// <inheritdoc />
        public IRecordStream<TTarget> Calls => CallsLocked;
        
        /// <inheritdoc />
        public new ISpy<TTarget> Via(IVia via, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Via(via, optionsAction);

            return this;
        }

        ISpy ISpy.Reset()
        {
            base.Reset();

            return this;
        }

        ISpy ISpy.Strict(bool? isStrict)
        {
            base.Strict(isStrict);

            return this;
        }

        ISpy ISpy.Via(IVia via, Action<IViaOptionsBuilder>? optionsAction)
        {
            base.Via(via, optionsAction);

            return this;
        }
        
        /// <inheritdoc />
        public new ISpy<TTarget> Reset()
        {
            base.Reset();

            return this;
        }
        
        /// <inheritdoc />
        public new ISpy<TTarget> Strict(bool? isStrict)
        {
            base.Strict(isStrict);

            return this;
        }
        
        /// <inheritdoc />
        public new ISpy<TTarget> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }
        
        protected override void ResetInternal()
        {
            ResetAndConfigureRecord();
        }
        
        private void ResetAndConfigureRecord()
        {
            var recordHandler = new RecordCallHandler<TTarget>();
            // Only record calls going to the Mock proxy
            var callConstraint = new DelegateCallConstraint<TTarget>(call => ReferenceEquals(call.Proxy, Mock));
            var via = ViaBuilder<TTarget>.To(callConstraint).Build(recordHandler);
            var options = ViaOptionsBuilder.Create(opt => opt.OrderFirst(), disableSatisfyStrict: true);
            RedirectRepository.ResetAndInsert(via, options);
            CallsLocked = recordHandler.RecordStream;
        }

        private RecordStream<TTarget> CallsLocked
        {
            get
            {
                lock (_callsLock)
                {
                    return _calls;
                }
            }

            set
            {
                lock (_callsLock)
                {
                    _calls = value;
                }
            }
        }
    }
    
    /// <summary>
    /// Static Spy helpers for creating spy mock objects and referencing spy instances from those mock objects.
    /// </summary>
    public static class Spy
    {
        /// <summary>
        /// Creates a spy of the given target type and returns its mock object.
        /// </summary>
        /// <returns>The spy mock object.</returns>
        public static TTarget On<TTarget>() where TTarget : class?
        {
            var spy = new Spy<TTarget>();

            return spy.Mock;
        }
        
        /// <summary>
        /// Creates a spy of the given target type and returns its mock object.
        /// </summary>
        /// <param name="root">The root instance the spy will wrap and relay calls to.</param>
        /// <returns>The spy mock object.</returns>
        public static TTarget On<TTarget>(TTarget? root) where TTarget : class?
        {
            var mock = new Spy<TTarget>(root);

            return mock.Mock;
        }

        /// <summary>
        /// Get the spy of a mock object.
        /// </summary>
        /// <param name="mock">The spy's mock object.</param>
        /// <returns>The spy instance.</returns>
        /// <exception cref="DiverterException">Thrown if if the given <paramref name="mock"/> object does not have an associated <see cref="ISpy{TTarget}"/> </exception>
        public static ISpy<TTarget> Of<TTarget>([DisallowNull] TTarget mock) where TTarget : class?
        {
            var redirect = Redirect.Of(mock);
            
            if (redirect is not Spy<TTarget> spyOf)
            {
                throw new DiverterException("Spy not found");
            }
            
            return spyOf;
        }
    }
}