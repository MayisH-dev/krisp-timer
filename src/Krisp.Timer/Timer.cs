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
        private readonly ConcurrentDictionary<CachingRequestToken, Unit> _scheduledSet = new();


        /// <inheritdoc />
        public RequestToken Start(Action<CancellationToken> callback, TimeSpan interval, int recurrence = ITimer.Once)
        {
            Throw.WhenNegative(recurrence, nameof(recurrence));

            RequestToken requestToken = new CachingRequestToken(_scheduledSet);

            requestToken.Schedule(
                callback,
                interval,
                recurrence);

            return requestToken;
        }

        /// <inheritdoc />
        public void Start(Action<CancellationToken> callback, RequestToken requestToken, TimeSpan interval, int recurrence = ITimer.Once)
        {
            Throw.WhenNegative(recurrence, nameof(recurrence));
            if (requestToken is CachingRequestToken token)
                ThrowWhenNotInCache(token);

            requestToken.Schedule(
                callback,
                interval,
                recurrence);
        }

        /// <inheritdoc />
        public void Cancel(RequestToken requestToken) => _ = requestToken.TryCancel();

        /// <inheritdoc />
        public void Stop()
        {
            foreach (var timerRequestToken in _scheduledSet.Keys)
                _ = timerRequestToken.TryCancel();
        }

        private void ThrowWhenNotInCache(CachingRequestToken requestToken)
        {
            if (!_scheduledSet.ContainsKey(requestToken))
                throw new InvalidOperationException("Attmpt to schedule an entangled callback which is not registered with the timer");
        }
    }
}