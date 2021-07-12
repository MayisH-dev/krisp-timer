using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Krisp.Timer.Private;

namespace Krisp.Timer
{
    public sealed class Timer : ITimer
    {
        /// <summary>
        /// A cache of tokens associated with currently scheduled callbacks
        /// </summary>
        /// <remarks>Needed for implementing the <see cref="Stop" /> operation</remarks>
        private readonly ConcurrentDictionary<TimerRequestToken, Unit> _scheduledSet = new();


        /// <inheritdoc />
        public TimerRequestToken Start(Action<CancellationToken> callback, TimeSpan interval, int recurrence = ITimer.Once)
        {
            Throw.WhenNegative(recurrence, nameof(recurrence));

            TimerRequestToken requestToken = new();

            requestToken.Register(() => _ = TryUncache(requestToken));

            if (_scheduledSet.TryAdd(requestToken, Unit.Value))
                ScheduleTaskPool(callback, interval, recurrence, requestToken.CancellationTokenSource.Token);

            return requestToken;
        }

        /// <inheritdoc />
        public void Start(Action<CancellationToken> callback, TimerRequestToken requestToken, TimeSpan interval, int recurrence = ITimer.Once)
        {
            Throw.WhenNegative(recurrence, nameof(recurrence));
            requestToken.ThrowWhenDisposed();
            ThrowWhenNotInCache(requestToken);

            ScheduleTaskPool(callback, interval, recurrence, requestToken.CancellationTokenSource.Token);
        }

        /// <inheritdoc />
        public void Cancel(TimerRequestToken requestToken)
        {
            if(requestToken.TryCancel())
                _ = TryUncache(requestToken);
        }

        /// <inheritdoc />
        public void Stop()
        {
            foreach(var timerRequestToken in _scheduledSet.Keys)
                timerRequestToken.TryCancel();

            _scheduledSet.Clear();
        }

        /// <summary>
        /// Schedules an operation in the task pool
        /// </summary>
        /// <param name="interval">The delay/interval between the operations</param>
        /// <param name="callback"></param>
        /// <param name="recurrence"></param>
        /// <param name="token"></param>
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

        private bool TryUncache(TimerRequestToken requestToken) => _scheduledSet.TryRemove(requestToken, out var _);

        private void ThrowWhenNotInCache(TimerRequestToken requestToken)
        {
            if(! _scheduledSet.ContainsKey(requestToken))
                throw new InvalidOperationException("Attmpt to schedule an entangled callback which is not registered with the timer");
        }
    }
}