using System;
using System.Linq;
using System.Threading.Tasks;
using Krisp.Timer;

// Showcases the non deterministic nature of Timer.Stop()
// In a highly concurrent system, the upside of synchronizing this operation will not have much upside anyways
// We just need Timer's underlying cache of request tokens to always stay consistent with the callbacks that were not cancelled by Stop()
// Thus, the outcome of this program greatly depends on the PLINQ partitioner(the size of the chunks which are scheduled for execution on the same OS thread)
ITimer timer = new Timer();

ParallelEnumerable.Range(1, 50)
    .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
    .ForAll(
        callbackId => {
            if(callbackId is 25)
                timer.Stop();
            else
                timer.Start(
                    _ => Console.WriteLine("Callback number: {0,2} has not been cancelled yet", callbackId),
                    TimeSpan.FromMilliseconds(100 + callbackId),
                    100
                );
        }
    );

await Task.Delay(100000);