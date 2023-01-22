using System;
using System.Diagnostics.CodeAnalysis;
using DivertR.Internal;
using DivertR.Record;
using DivertR.Record.Internal;

namespace DivertR
{
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
            Spy.AddSpy(this, Mock);
            ResetAndConfigureRecord(false);
        }
        
        [NotNull]
        public TTarget Mock { get; }

        IRecordStream ISpy.Calls => CallsLocked;

        object ISpy.Mock => Mock;

        public IRecordStream<TTarget> Calls => CallsLocked;

        public new ISpy<TTarget> Via(IVia via, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Via(via, optionsAction);

            return this;
        }

        ISpy ISpy.Reset(bool includePersistent)
        {
            base.Reset(includePersistent);

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

        public new ISpy<TTarget> Reset(bool includePersistent = false)
        {
            base.Reset(includePersistent);

            return this;
        }

        public new ISpy<TTarget> Strict(bool? isStrict)
        {
            base.Strict(isStrict);

            return this;
        }

        public new ISpy<TTarget> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null)
        {
            base.Retarget(target, optionsAction);

            return this;
        }
        
        protected override void ResetInternal(bool includePersistent)
        {
            ResetAndConfigureRecord(includePersistent);
        }
        
        private void ResetAndConfigureRecord(bool includePersistent)
        {
            var recordHandler = new RecordCallHandler<TTarget>();
            // Only record calls going to the Mock proxy
            var callConstraint = new CallConstraint<TTarget>(call => ReferenceEquals(call.Proxy, Mock));
            var via = ViaBuilder<TTarget>.To(callConstraint).Build(recordHandler);
            var options = ViaOptionsBuilder.Create(opt => opt.OrderFirst(), disableSatisfyStrict: true);
            RedirectRepository.Reset(via, options, includePersistent);
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

    public static class Spy
    {
        private static readonly SpyTracker SpyTracker = new();
        
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
        public static ISpy<TTarget> Of<TTarget>([DisallowNull] TTarget mock) where TTarget : class?
        {
            return SpyTracker.GetSpy<TTarget>(mock);
        }

        internal static void AddSpy<TTarget>(Spy<TTarget> spy, [DisallowNull] TTarget mock) where TTarget : class?
        {
            SpyTracker.AddSpy(spy, mock);
        }
    }
}