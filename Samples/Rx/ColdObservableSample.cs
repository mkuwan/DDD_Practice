using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;
using Xunit;

namespace Samples.Rx
{
    // ColdなObservableは「複数回Subscribeした時に、Observerごとに独立した値を発行する」

    // cf) HotなObservableは「複数回Subscribeしたときに、全てのObserverに同じタイミングで同じ値を発行する」

    public interface IObservableSource<T>
    {
        IObservable<T> Source { get; }
        void Execute(T value);
    }

    internal class ObservableSource<T>: IObservableSource<T>
    {
        private readonly Subject<T> _source = new Subject<T>();

        public IObservable<T> Source => _source.AsObservable();

        public void Execute(T value) => _source.OnNext(value);
    }
    

    /// <summary>
    /// Rxでは OnCompleted()が呼ばれると自動でDisposeが行われます。
    /// そのため、OnCompleted()がある場合は、後始末としてIDisposal.Dispose()は不要です
    /// ただ、バグを避けるためにあえてDisposeを呼んでもいいかもしれませんが、不要なものを作成するルールはかえって意味がないかも
    /// </summary>
    public class ReactiveExtensionsTest
    {
        /// <summary>
        /// 上記のInterface, 実装のテスト
        /// </summary>
        [Fact]
        public void ObservableSourceTest()
        {
            string value = String.Empty;

            IObservableSource<string> source = new ObservableSource<string>();
            source.Source.Subscribe(x => value = x + " Mika");

            source.Execute("Arahata");

            Assert.Equal("Arahata Mika", value);
            
        }


        /// <summary>
        /// Rxの動作確認
        /// 上のクラスは使用していません
        /// </summary>
        [Theory]
        [InlineData(100, 200)]
        [InlineData(500, 1000)]
        public void SubjectTest(int value, int expected)
        {
            // Arrange
            ISubject<int> subject = new Subject<int>();
            IObservable<int> observable = subject.AsObservable();

            int checkValue = 0;
            var disposer = observable.Subscribe(x => checkValue = x * 2);

            // Act
            subject.OnNext(value);

            // Assertion
            Assert.Equal(expected, checkValue);

            // dispose
            checkValue = 0;
            disposer.Dispose();
            subject.OnNext(value);
            Assert.Equal(0, checkValue);

        }

        [Fact]
        public void ObservablePracticeTest()
        {
            var publisher = new EventPublisher();

            var value = 0;
            var subscription = publisher.Observable.Subscribe(x => value += x);

            publisher.OnRaised(1);
            publisher.OnRaised(3);

            Assert.Equal(4, value);
            subscription.Dispose();
        }

        class EventPublisher
        {
            private readonly ISubject<int> subject;
            public readonly IObservable<int> Observable;

            public EventPublisher()
            {
                subject = new Subject<int>();
                Observable = subject.AsObservable();
            }

            public void OnRaised(int value)
            {
                subject.OnNext(value);
            }
        }

        [Fact]
        public void ObservablePracticeTest2()
        {
            var publisher = new EventPublisher();
            EventSubscriber subscriber = new EventSubscriber(publisher);

            subscriber.DoWork(10);
            subscriber.DoWork(2);

            Assert.Equal(12, subscriber.Value);

        }

        class EventSubscriber
        {
            private readonly EventPublisher _publisher;
            private readonly IDisposable disposable;
            public int Value { get; private set; } = 0;

            public EventSubscriber(EventPublisher publisher)
            {
                _publisher = publisher;
                disposable = _publisher.Observable.Subscribe(x => Value += x);
            }

            public void DoWork(int value)
            {
                _publisher.OnRaised(value);
            }

        }



        [Fact]
        public void ObservableReturnTest()
        {
            string result = String.Empty;

            // 10を返すIObservable<int>を作成
            var source = Observable.Return(10);

            // 購読
            var subscription = source.Subscribe(n => result = n.ToString());
            
            Assert.Equal("10", result);

            subscription.Dispose();
        }

        [Fact]
        public void ObservableRepeatTest()
        {
            int result = 0;
            
            // 2を5回発行するIObservable<int>を作成
            var source = Observable.Repeat(2, 5);

            var subscription = source.Subscribe(n => result += n);

            Assert.Equal(10, result);

            subscription.Dispose();
        }

        [Fact]
        public void ObservableRangeTest()
        {
            int result = 0;

            // 1から始まる値を10個発行する
            var source = Observable.Range(1, 10);

            var subscription = source.Subscribe(n => result += n);

            Assert.Equal(55, result);

            subscription.Dispose();
        }

        [Fact]
        public void ObservableGenerateTest()
        {
            int result = 0;
            int expected = 0;
            for (int i = 0; i < 10; i++)
            {
                expected += i * i;
            }

            // for(int i = 0; i < 10; i++) { yield return i * i} のようなイメージ
            var source = Observable.Generate(0, i => i < 10, i => ++i, i => i * i);

            var subscription = source.Subscribe(n => result += n);

            Assert.Equal(expected, result);

            subscription.Dispose();
        }


        /// <summary>
        /// DeferはIObservable<T>を直接返すラムダ式を引数に渡し、Subscribeの度にDeferメソッドが実行されてIObservable<T>が作成される
        /// </summary>
        [Fact]
        public void ObservableDeferTest()
        {
            int result = 0;

            var source = Observable.Defer<int>(() =>
            {
                var subject = new ReplaySubject<int>();
                subject.OnNext(100);
                subject.OnNext(10);
                subject.OnNext(1);
                return subject.AsObservable();
            });

            var subscription = source.Subscribe(n => result += n);

            Assert.Equal(111, result);
        }

        /// <summary>
        /// Createは「IObserver<T>を受け取りActionを返すラムダ式」を引数とします
        /// また、returnでDispose時の処理を行うことができます
        /// </summary>
        [Fact]
        public void ObservableCreateTest()
        {
            int result = 0;
            bool callCompleted = false;

            var source = Observable.Create<int>(observer =>
            {
                // 引数のIObserver<int>に対してOn***メソッドを呼ぶ
                observer.OnNext(1);
                observer.OnNext(2);
                observer.OnNext(3);

                // OnCompletedが呼ばれるとDisposeが自動的に呼ばれます
                // スコープが外のものを使用していて本来は好ましくはありません。動作確認のためのみです。本当はテストを二種類作るものです
                if(callCompleted)
                    observer.OnCompleted();

                // Disposeが呼ばれた時の処理
                return () =>
                {
                    result = 0;
                };
            });

            var subscription = source.Subscribe(
                i => result += i,
                err => Console.WriteLine("OnError({0})", err.Message),
                () => Console.WriteLine("Completed")
            );

            Assert.Equal(6, result);

            callCompleted = true;
            subscription = source.Subscribe(
                i => result += i
            );

            Assert.Equal(0, result);
        }

        [Fact]
        public void ObservableUsingTest()
        {
            var result = String.Empty;

            var source = Observable.Using(
                () => new SampleResource(),
                s => s.GetData()
            );

            var subscription = source.Subscribe(
                s => result += s,
                err => Console.WriteLine("OnError({0})", err.Message),
                () => {}
            );

            Assert.Equal("My name is Mika", result);
        }

        class SampleResource : IDisposable
        {
            public IObservable<string> GetData()
            {
                return Observable.Create<string>(observer =>
                {
                    observer.OnNext("My ");
                    observer.OnNext("name is ");
                    observer.OnNext("Mika");

                    return Disposable.Empty;
                });
            }

            public void Dispose()
            {
                Console.WriteLine("Resource Dispose Called");
            }
        }

        [Fact]
        public void ObservableTimerTest()
        {
            int count = 0;
            // 2秒後に1秒間隔で実行
            var source = Observable.Timer(
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(1)
            );

            var subscription = source.Subscribe(
                i => Console.WriteLine(count++),
                err => Console.WriteLine("OnError:{0}", err.Message),
                () => Console.WriteLine("Completed")
            );

            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < 4500)
            {
                // 2sec,3sec,4secで実行されるはず
            }
            sw.Stop();
            subscription.Dispose();

            Assert.Equal(3, count);
        }

        [Fact]
        public void ObservableIntervalTest()
        {
            // 1秒間隔で実行する
            var source = Observable.Interval(TimeSpan.FromSeconds(1));

            int count = 0;
            var subscription = source.Subscribe(
                i => Console.WriteLine(count++),
                err => Console.WriteLine("OnError:{0}", err.Message),
                () => Console.WriteLine("Completed"));

            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (sw.ElapsedMilliseconds < 4500)
            {
                //1sec,2sec,3sec,4secで実行されるはず
            }
            sw.Stop();
            subscription.Dispose();
            Assert.Equal(4, count);
        }
    }


}
