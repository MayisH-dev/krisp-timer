using System;
using System.Threading;
using Xunit;

namespace Krisp.Timer.UnitTests
{
    public sealed class TimerTests
    {
        #region Start Tests
        [Fact]
        public void Start_ExecutesCallback()
        {
            Timer timer = new();
            bool flag = false;

            _ = timer.Start(_ => flag = true, TimeSpan.FromMilliseconds(10), ITimer.Once);
            Thread.Sleep(20);

            Assert.True(flag);
        }

        [Fact]
        public void Start_ExecutesMultipleTimes()
        {
            Timer timer = new();
            const int initial = 0;
            int accumulator = initial;

            const int Recurrence = 2;
            _ = timer.Start(_ => accumulator++, TimeSpan.FromMilliseconds(10), recurrence: Recurrence);

            Thread.Sleep(45);

            Assert.Equal(initial + Recurrence, accumulator);
        }

        #endregion
        #region Cancel Tests

        [Fact]
        public void Cancel_CancelsCallback()
        {
            Timer timer = new();
            bool flag = false;

            var Timer = timer.Start(_ => flag = true, TimeSpan.FromMilliseconds(10), ITimer.Once);
            timer.Cancel(Timer);
            Thread.Sleep(20);

            Assert.False(flag);
        }

        [Fact]
        public void Cancel_DoesntCancelOtherCallbacks()
        {
            Timer timer = new();
            bool flag = false;

            _ = timer.Start(_ => flag = true, TimeSpan.FromMilliseconds(10), ITimer.Once);
            var Timer = timer.Start(_ => { }, TimeSpan.FromMilliseconds(10));

            timer.Cancel(Timer);
            Thread.Sleep(20);

            Assert.True(flag);
        }

        #endregion
        #region Stop Tests

        [Fact]
        public void Stop_CancelsAllCallbacks()
        {
            Timer timer = new();
            bool flag = false;

            _ = timer.Start(_ => flag = true, TimeSpan.FromMilliseconds(10), ITimer.Once);
            _ = timer.Start(_ => flag = true, TimeSpan.FromMilliseconds(10), ITimer.Once);
            timer.Stop();
            Thread.Sleep(40);

            Assert.False(flag);
        }

        #endregion
    }
}