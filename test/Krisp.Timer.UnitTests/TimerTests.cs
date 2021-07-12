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

            _ = timer.Start(_ => flag = true, TimeSpan.FromMilliseconds(10));
            Thread.Sleep(40);

            Assert.True(flag);
        }

        [Fact]
        public void Start_ExecutesMultipleTimes()
        {
            Timer timer = new();
            const int initial = 0;
            int accumulator = initial;

            const int Recurrence = 2;
            _ = timer.Start(_ => accumulator++, TimeSpan.Zero, recurrence: Recurrence);

            Thread.Sleep(20);

            Assert.Equal(initial + Recurrence, accumulator);
        }

        [Fact]
        public void Start_ExecutesEntangledCallback()
        {
            Timer timer = new();
            bool flag = false;

            var token = timer.Start(_ => { }, TimeSpan.Zero);
            timer.Start(_ => flag = true, token, TimeSpan.Zero);

            Thread.Sleep(20);

            Assert.True(flag);
        }

        #endregion
        #region Cancel Tests

        [Fact]
        public void Cancel_CancelsCallback()
        {
            Timer timer = new();
            bool flag = false;

            var requestToken = timer.Start(_ => flag = true, TimeSpan.FromMilliseconds(10));
            timer.Cancel(requestToken);
            Thread.Sleep(20);

            Assert.False(flag);
        }

        [Fact]
        public void Cancel_CancelsEntangledCallbacks()
        {
            Timer timer = new();
            bool flag = false;

            var requestToken = timer.Start(_ => flag = true, TimeSpan.FromMilliseconds(10));
            timer.Start(_ => flag = true, requestToken, TimeSpan.FromMilliseconds(10));
            timer.Cancel(requestToken);
            Thread.Sleep(40);

            Assert.False(flag);
        }

        [Fact]
        public void Cancel_DoesntCancelOtherCallbacks()
        {
            Timer timer = new();
            bool flag = false;

            _ = timer.Start(_ => flag = true, TimeSpan.FromMilliseconds(10));
            var requestToken = timer.Start(_ => { }, TimeSpan.FromMilliseconds(10));

            timer.Cancel(requestToken);
            Thread.Sleep(30);

            Assert.True(flag);
        }

        #endregion
        #region Stop Tests

        [Fact]
        public void Stop_CancelsAllCallbacks()
        {
            Timer timer = new();
            bool flag = false;

            _ = timer.Start(_ => flag = true, TimeSpan.FromMilliseconds(10));
            _ = timer.Start(_ => flag = true, TimeSpan.FromMilliseconds(10));
            timer.Stop();
            Thread.Sleep(40);

            Assert.False(flag);
        }

        #endregion
    }
}