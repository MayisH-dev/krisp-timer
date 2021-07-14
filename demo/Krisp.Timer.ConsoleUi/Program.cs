using System;
using System.Linq;
using System.Threading.Tasks;
using Krisp.Timer;

ITimer timer = new Krisp.Timer.Timer();
ParallelEnumerable.Range(1,20)
    .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
    .ForAll(num =>
    {
        int iter = 1;
        timer.Start(_ =>
            Console.WriteLine(
                "Now: {3}, Scheduled callback Id: {0,2}, Printing every {1,3} milliseconds, print count: {2,6}",
                num,
                num * 5,
                iter++,
                DateTime.Now.Ticks),
            TimeSpan.FromMilliseconds(num * 5),
            ITimer.UnlimitedRecurrence);
    });


await Task.Delay(100000);