using System;

namespace Krisp.Timer
{
    public sealed class Timer : ITimer
    {
        public TimerRequestToken Start(TimeSpan interval, Action callback, int recurrence = ITimer.UnlimitedRecurrence)
        {
            throw new NotImplementedException();
        }

        public void Cancel(TimerRequestToken request)
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}