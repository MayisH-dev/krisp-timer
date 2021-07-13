using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Krisp.Timer.Private;

namespace Krisp.Timer
{
    /// <summary>
    /// An opaque type representing an identifier of a scheduled operation
    /// </summary>
    public sealed class TimerRequestToken : IDisposable
    {
        [MemberNotNullWhen(false, nameof(_cancellationTokenSource))]
        private bool IsDisposed { set; get; }

        private Action? _disposalCallbacks = null;

        private CancellationTokenSource? _cancellationTokenSource;

        private readonly object _sync = new();

        internal TimerRequestToken()
        {
            _cancellationTokenSource = new();
        }

        internal void Schedule(
            Action<CancellationToken> scheduledCallback,
            TimeSpan interval,
            int recurrence,
            Action? precedingCallback = null,
            Action? disposalCallback = null,
            Action? cancellationCallback = null)
        {
            Throw.WhenDisposed(IsDisposed, nameof(TimerRequestToken));

            lock(_sync)
            {
                Throw.WhenDisposed(IsDisposed, nameof(TimerRequestToken));

                _disposalCallbacks += disposalCallback;
                precedingCallback?.Invoke();
                ScheduleTaskPool(scheduledCallback, interval, recurrence, _cancellationTokenSource.Token);
                if(cancellationCallback is not null)
                    _ = _cancellationTokenSource.Token.Register(cancellationCallback);
            }
        }

        /// <summary>
        /// Schedules an operation in the task pool
        /// </summary>
        /// <param name="interval">The delay/interval between the operations</param>
        /// <param name="callback">The cancellable callback</param>
        /// <param name="recurrence">The number of times the callback is invoked</param>
        /// <param name="token">The cancellation token</param>
        private static void ScheduleTaskPool(
            Action<CancellationToken> callback,
            TimeSpan interval,
            int recurrence,
            CancellationToken token = default) =>
            Task.Run(
                async () =>
                {
                    if (recurrence == ITimer.UnlimitedRecurrence)
                        while (true)
                        {
                            await Task.Delay(interval, token);
                            callback(token);
                        }
                    else
                        for (int i = 0; i < recurrence; ++i)
                        {
                            await Task.Delay(interval, token);
                            callback(token);
                        }
                },
                token);

        /// <summary>
        /// Attempts to cancel the scheduled callbacks associated with this token
        /// </summary>
        /// <returns><c>true</c> if cancellation was successful, <c>false</c> otherwise</returns>
        internal bool TryCancel()
        {
            if(IsDisposed)
                return false;

            lock (_sync)
            {
                if(IsDisposed || _cancellationTokenSource is null || _cancellationTokenSource.IsCancellationRequested)
                    return false;

                _cancellationTokenSource.Cancel();
                return true;
            }
        }

        /// <summary>
        /// Dispose the resources associated with the <see cref="TimerRequestToken" />
        /// </summary>
        /// <remarks>A disposed request cannot be used to cancel the associated scheduled task</remarks>
        public void Dispose()
        {
            if(IsDisposed) return;

            lock (_sync)
            {
                if (IsDisposed) return;

                _disposalCallbacks?.Invoke();
                _disposalCallbacks = null;
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }
    }
}