using System;
using System.Collections.Concurrent;
using System.Threading;
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

            requestToken.Schedule(
                callback,
                interval,
                recurrence,
                AddToCache,
                RemoveFromCache,
                RemoveFromCache);

            return requestToken;

            void AddToCache() => _ = _scheduledSet.TryAdd(requestToken, Unit.Value);
            void RemoveFromCache() => _ = _scheduledSet.TryRemove(requestToken, out var _);
        }

        /// <inheritdoc />
        public void Start(Action<CancellationToken> callback, TimerRequestToken requestToken, TimeSpan interval, int recurrence = ITimer.Once)
        {
            Throw.WhenNegative(recurrence, nameof(recurrence));
            ThrowWhenNotInCache(requestToken);

            requestToken.Schedule(
                callback,
                interval,
                recurrence,
                disposalCallback: RemoveFromCache,
                cancellationCallback: RemoveFromCache
            );

            void RemoveFromCache() => _ = _scheduledSet.TryRemove(requestToken, out var _);
        }

        /// <inheritdoc />
        public void Cancel(TimerRequestToken requestToken) => requestToken.TryCancel();

        /// <inheritdoc />
        public void Stop()
        {
            foreach (var timerRequestToken in _scheduledSet.Keys)
                timerRequestToken.TryCancel();
        }

        private void ThrowWhenNotInCache(TimerRequestToken requestToken)
        {
            if (!_scheduledSet.ContainsKey(requestToken))
                throw new InvalidOperationException("Attmpt to schedule an entangled callback which is not registered with the timer");
        }
    }
}