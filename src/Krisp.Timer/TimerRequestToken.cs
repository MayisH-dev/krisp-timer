using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Krisp.Timer.Private;

namespace Krisp.Timer
{
    /// <summary>
    /// An opaque type representing an identifier of a scheduled operation
    /// </summary>
    public sealed class TimerRequestToken : IDisposable
    {
        private const bool FromFinalizer = false;
        private const bool FromDispose = true;

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

        internal bool TryCancel()
        {
            if (IsDisposed && CancellationTokenSource.IsCancellationRequested)
                return false;
            CancellationTokenSource.Cancel();
            return true;
        }

        public void Dispose()
        {
            if(IsDisposed)
                return;
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
    }
}