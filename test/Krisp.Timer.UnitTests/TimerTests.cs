using System.Threading;
using Xunit;

namespace Krisp.Timer.UnitTests
{
    public sealed class TimerTests
    {
        [Fact]
        public void Start_ExecutesCallback()
        {
            Timer timer = new();
            bool flag = false;

            _ = timer.Start(System.TimeSpan.FromMilliseconds(10), () => flag = true, ITimer.Once);
            Thread.Sleep(20);

            Assert.True(flag);
        }

        [Fact]
        public void Cancel_CancelsCallback()
        {
            Timer timer = new();
            bool flag = false;

            var token = timer.Start(System.TimeSpan.FromMilliseconds(10), () => flag = true, ITimer.Once);
            timer.Cancel(token);
            Thread.Sleep(20);

            Assert.False(flag);
        }

        [Fact]
        public void Start_ExecutesMultipleTimes()
        {
            Timer timer = new();
            const int initial = 0;
            int accumulator = initial;

            const int MillisecondDelay = 10;
            _ = timer.Start(System.TimeSpan.FromMilliseconds(MillisecondDelay), () => accumulator++, recurrence: 2);

            const int MillisecondsTimeout = 25;
            Thread.Sleep(MillisecondsTimeout);

            Assert.Equal(initial + MillisecondsTimeout / MillisecondDelay, accumulator);
        }

        [Fact]
        public void Stop_CancelsAllCallbacks()
        {
            Timer timer = new();
            bool flag1 = false;
            bool flag2 = false;

            _ = timer.Start(System.TimeSpan.FromMilliseconds(10), () => flag1 = true, ITimer.Once);
            _ = timer.Start(System.TimeSpan.FromMilliseconds(10), () => flag2 = true, ITimer.Once);
            Thread.Sleep(20);

            Assert.False(flag1 || flag2);
        }
    }
}