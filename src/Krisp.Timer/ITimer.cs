using System;
using System.Threading;

namespace Krisp.Timer
{
    public interface ITimer
    {
        public const int UnlimitedRecurrence = 0;
        public const int Once = 1;

        /// <summary>
        /// Schedules a callback to be invoked at a later time, possibly multiple times
        /// </summary>
        /// <param name="interval">The time span the callback should be invoked in</param>
        /// <param name="callback">The callback</param>
        /// <param name="recurrence">How many times the callback should be invoked</param>
        /// <returns>A request token associated with the scheduled task</returns>
        TimerRequestToken Start(TimeSpan interval, Action callback, int recurrence = UnlimitedRecurrence);

        /// <summary>
        /// Cancels a scheduled task, or does nothing if it has already been cancelled
        /// </summary>
        /// <param name="request">The request token associated with the scheduled task</param>
        void Cancel(TimerRequestToken request);

        /// <summary>
        /// Cancelled all scheduled tasks
        /// </summary>
        void Stop();
    }
}