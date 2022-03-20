using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Samples.Rx
{
    // HotなObservableは「複数回Subscribeしたときに、全てのObserverに同じタイミングで同じ値を発行する」

    // cf) ColdなObservableは「複数回Subscribeした時に、Observerごとに独立した値を発行する」

    public class HotObservableSample
    {
        //public static IObservable<TEventArgs> FromEvent<TDelegate, TEventArgs>(
        //    // Action<TEventArgs>からイベントハンドラの形へと変換する処理
        //    // Action<TEventArgs>はSubscribeしたときのOnNextの処理にあたる。
        //    Func<Action<TEventArgs>, TDelegate> conversion,
        //    // イベントハンドラを登録する処理
        //    Action<TDelegate> addHandler,
        //    // イベントハンドラの登録を解除する処理
        //    Action<TDelegate> removeHandler);
        [Fact]
        public void ObservableFromEventTest()
        {
            var eventSource = new EventSource();
            var source = Observable.FromEvent<EventHandler, EventArgs>(
                h => eventSource.Raise += h,
                h => eventSource.Raise -= h
            );

            var subscription1 = source.Subscribe(
                x => { },
                err => Console.WriteLine($"{err.Message}"),
                () => {}
                );

            eventSource.OnRaise(2);
            eventSource.OnRaise(3);
            eventSource.OnRaise(4);

            Assert.Equal(3, eventSource.EventCount);
            Assert.Equal(9, eventSource.ParamCount);
            
            subscription1.Dispose();
        }

        class EventSource
        {
            public event EventHandler? Raise;

            public int EventCount { get; private set; }
            public int ParamCount { get; private set; }

            public void OnRaise(int value)
            {
                var h = this.Raise;
                h?.Invoke(this, EventArgs.Empty);

                EventCount++;
                ParamCount += value;
            }
        }


    }


}
