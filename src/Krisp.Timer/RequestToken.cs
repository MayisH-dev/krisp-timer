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
    public abstract class RequestToken : IDisposable
    {
        [MemberNotNullWhen(false, nameof(_cancellationTokenSource))]
        private bool IsDisposed { set; get; }

        private CancellationTokenSource? _cancellationTokenSource;

        private readonly object _sync = new();

        /// <summary>
        /// Defines pre-dispose behavior for inherited types
        /// </summary>
        /// <remarks>
        /// This call is protected against disposed state
        /// </remarks>
        private protected abstract void OnDispose();

        /// <summary>
        /// Defines schedule behavior for inherited types
        /// </summary>
        /// <remarks>
        /// This call is protected against disposed state
        /// </remarks>
        private protected abstract void OnSchedule();

        /// <summary>
        /// Defines cancellation behavior for inherited types
        /// </summary>
        /// <remarks>
        /// This call is protected against disposed state
        /// </remarks>
        private protected abstract void OnCancel();

        /// <summary>
        /// Defines completion behavior for inherited types
        /// </summary>
        /// <remarks>
        /// This call is not protected against disposed state
        /// </remarks>
        private protected abstract void OnComplete();

        private protected RequestToken()
        {
            _cancellationTokenSource = new();
        }

        /// <summary>
        /// Dispose the resources associated with the <see cref="RequestToken" />
        /// </summary>
        /// <remarks>A disposed request cannot be used to cancel the associated scheduled task</remarks>
        public void Dispose()
        {
            if(IsDisposed) return;

            lock (_sync)
            {
                if (IsDisposed) return;

                OnDispose();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
                IsDisposed = true;
            }
        }

        /// <summary>
        /// Schedules an operation in the task pool
        /// </summary>
        /// <param name="interval">The delay/interval between the operations</param>
        /// <param name="callback">The cancellable callback</param>
        /// <param name="recurrence">The number of times the callback is invoked</param>
        /// <param name="token">The cancellation token</param>
        private void ScheduleTaskPool(
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
                    OnComplete();
                },
                token);

        /// <summary>
        /// Schedules a callback to be executed in background
        /// </summary>
        /// <remarks>This operation is atomic and thread safe</remarks>
        /// <param name="scheduledCallback">The callback to be scheduled</param>
        /// <param name="interval">The interval/delay before/between the callbacks are executed</param>
        /// <param name="recurrence">Number of times the callback is executed</param>
        /// <param name="precedingCallback">An optional callback that will be executed before scheduling the callback</param>
        /// <param name="disposalCallback">An optional callback that will be executed upon disposal of the request token</param>
        /// <param name="cancellationCallback">An optional callback that will be executed before cancelling the callback</param>
        internal void Schedule(
            Action<CancellationToken> scheduledCallback,
            TimeSpan interval,
            int recurrence)
        {
            Throw.WhenDisposed(IsDisposed, nameof(CachingRequestToken));

            lock(_sync)
            {
                Throw.WhenDisposed(IsDisposed, nameof(CachingRequestToken));

                OnSchedule();
                _ = _cancellationTokenSource.Token.Register(OnCancel);
                ScheduleTaskPool(scheduledCallback, interval, recurrence, _cancellationTokenSource.Token);
            }
        }

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
    }
}