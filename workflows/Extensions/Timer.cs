using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Reactive.Linq;
using System.Reactive;
using System.Diagnostics;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Source)]
public class Timer
{
    public double MiliSeconds { get; set; }
    public IObservable<int> Process()
    {
        return Observable.Create<int>((observer, cancellationToken) =>
        {
            return Task.Factory.StartNew(() =>
            {
                AutoResetEvent autoEvent = new AutoResetEvent(false);
                Stopwatch busyCrono = new Stopwatch();
                int count = 1;
                double interval = (MiliSeconds*Stopwatch.Frequency/1000);
                double timeCouter= (Stopwatch.GetTimestamp() + interval);
                while (!cancellationToken.IsCancellationRequested)
                {
                    var time = (timeCouter-Stopwatch.GetTimestamp())/Stopwatch.Frequency*1000;
                     if (time > 0)
                     {
                         autoEvent.WaitOne((int)time);//.WaitOne(1);// (int)(time/(Stopwatch.Frequency*1000)));
                     }
                    timeCouter += interval;
                    observer.OnNext(count);
                    count++;
                }
            },
            cancellationToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
        }).PublishReconnectable().RefCount();
    }
}
