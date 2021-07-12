using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Krisp.Timer.Private;

namespace Krisp.Timer
{
    /// <summary>
    /// An opaque type representing an identifier of a scheduled operation
    /// </summary>
    /// <remarks>
    /// Objects of this type are not intended to be shared from multiple threads
    /// </remarks>
    public sealed class TimerRequestToken : IDisposable
    {
        [MemberNotNullWhen(false, nameof(_cancellationTokenSource))]
        private bool IsDisposed { set; get; }

        [AllowNull, MaybeNull]
        private CancellationTokenSource _cancellationTokenSource;

        internal TimerRequestToken()
        {
            _cancellationTokenSource = new();
        }

        internal CancellationTokenSource CancellationTokenSource
        {
            get
            {
                Throw.WhenDisposed(IsDisposed, nameof(TimerRequestToken));
                return _cancellationTokenSource;
            }
        }

        /// <summary>
        /// Register <paramref name = "callback" /> to be executed after the timer request token is cancelled
        /// </summary>
        /// <param name="callback">The callback to be executed</param>
        internal void Register(Action callback) => _ = CancellationTokenSource.Token.Register(callback);

        internal bool TryCancel()
        {
            if (IsDisposed && CancellationTokenSource.IsCancellationRequested)
                return false;
            CancellationTokenSource.Cancel();
            return true;
        }

        internal void ThrowWhenDisposed()
        {
            Throw.WhenDisposed(IsDisposed, nameof(TimerRequestToken));
        }

        /// <summary>
        /// Dispose the resources associated with the <see cref="TimerRequestToken" />
        /// </summary>
        /// <remarks>A disposed request cannot be used to cancel the associated scheduled task</remarks>
        public void Dispose()
        {
            if(IsDisposed)
                return;
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }
}