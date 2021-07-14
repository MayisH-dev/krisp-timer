using System;
using System.Linq;
using System.Threading.Tasks;
using Krisp.Timer;

// Showcases the non deterministic nature of Timer.Stop()
// In a highly concurrent system, the upside of synchronizing this operation will not have much upside as

ITimer timer = new Krisp.Timer.Timer();

ParallelEnumerable.Range(1, 10)
    .ForAll(
        num => {
            if(num is 4)
                timer.Stop();
            else
                timer.Start(
                    _ => Console.WriteLine("Callback number: {0,2} has not been cancelled yet", num),
                    TimeSpan.FromMilliseconds(100 + num),
                    100
                );
        }
    );

await Task.Delay(100000);