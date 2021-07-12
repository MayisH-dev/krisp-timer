using System;
using System.Threading;

namespace Krisp.Timer
{
    public interface ITimer
    {
        public const int UnlimitedRecurrence = 0;
        public const int Once = 1;

        /// <summary>
        /// Schedules <paramref name="callback" /> to be invoked at a later time, possibly multiple times
        /// </summary>
        /// <param name="callback">The cancellable <paramref name="callback" /></param>
        /// <param name="interval">The time span delay/interval the <paramref name="callback" /> should be invoked in</param>
        /// <param name="recurrence">How many times the <paramref name="callback" /> should be invoked</param>
        /// <returns>A <see cref="TimerRequestToken" /> associated with the scheduled callback</returns>
        TimerRequestToken Start(Action<CancellationToken> callback, TimeSpan interval, int recurrence = Once);

        /// <summary>
        /// Schedules <paramref name="callback" /> to be invoked at a later time, possibly multiple times, with cancellation entangled to an existing request
        /// </summary>
        /// <param name="callback">The cancellable callback</param>
        /// <param name ="requestToken">The request token to be associated with the scheduled <paramref name="callback" /></param>
        /// <param name="interval">The time span delay/interval the <paramref name="callback" /> should be invoked in</param>
        /// <param name="recurrence">How many times the <paramref name="callback" /> should be invoked</param>
        void Start(Action<CancellationToken> callback, TimerRequestToken requestToken, TimeSpan interval, int recurrence = Once);

        /// <summary>
        /// Cancels all scheduled callbacks associated with <paramref name="request" />
        /// </summary>
        /// <param name="request">The request token associated with the scheduled callbacks that need to be cancelled</param>
        void Cancel(TimerRequestToken request);

        /// <summary>
        /// Cancells all scheduled callbacks
        /// </summary>
        void Stop();
    }
}