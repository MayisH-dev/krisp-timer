using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Krisp.Timer
{
    public sealed class Timer : ITimer
    {

        public TimerRequestToken Start(TimeSpan interval, Action callback, int recurrence = ITimer.UnlimitedRecurrence)
        {
            TimerRequestToken requestToken = new(new CancellationTokenSource());
            Task.Run(
                async () =>
                {
                    await Task.Delay(interval);
                    callback();
                },
                requestToken.CancellationTokenSource.Token);
            return requestToken;
        }

        public void Cancel(TimerRequestToken requestToken)
        {
            requestToken.CancellationTokenSource.Cancel();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}