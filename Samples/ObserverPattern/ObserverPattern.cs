using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Samples.ObserverPattern
{
    /// <summary>
    /// Observable
    /// データを発行するクラス
    /// </summary>
    internal class Observable : IObservable<int>
    {
        private readonly List<IObserver<int>> _observers = new List<IObserver<int>>();

        public IDisposable Subscribe(IObserver<int> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }

            //購読解除用のクラスをIDisposableとして返す
            return new UnSubscriber(_observers, observer);
        }

        public void Execute(int value)
        {
            foreach (IObserver<int> observer in _observers)
            {
                observer.OnNext(value);
            }
        }

        /// <summary>
        /// 購読解除用IDisposable実装
        /// </summary>
        internal class UnSubscriber : IDisposable
        {
            private List<IObserver<int>>? _observers = new();
            private IObserver<int>? _observer;

            internal UnSubscriber(List<IObserver<int>> observers, IObserver<int> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if(_observer == null) return;
                
                if(_observers?.IndexOf(_observer) != -1)
                    _observers?.Remove(_observer);

                _observers = null;
                _observer = null;
            }
        }
    }

    /// <summary>
    /// Observer
    /// 監視する側
    /// Observableの状態を監視している
    /// データを受け取るクラス
    /// </summary>
    internal class Observer : IObserver<int>
    {
        private readonly ILogger _logger;

        public string Message { get; private set; } = String.Empty;

        public Observer()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddJsonConsole(options => options.IncludeScopes = true);
            });
            _logger = loggerFactory.CreateLogger<Observer>();
        }

        /// <summary>
        /// データの発行が完了したことを通知する
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void OnCompleted()
        {
            Message = $"Observer: 通知の受け取りが完了しました";
            _logger.LogInformation(Message);
        }

        /// <summary>
        /// データの発行元でエラーが発生したことを通知する
        /// </summary>
        /// <param name="error"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnError(Exception error)
        {
            Message = $"Observer: エラーを受信しました ${error.Message}";
            _logger.LogInformation(Message);
        }

        /// <summary>
        /// 変更通知があったときに呼ばれるメソッド
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnNext(int value)
        {
            Message = $"Observer: {value}を受信しました";
            _logger.LogInformation(Message);
        }
    }


    public class ObserverPatternTest
    {
        private int MultipliedBy100(int value) => value * 100;

        [Theory]
        [InlineData(10, 1000)]
        [InlineData(0, 0)]
        [InlineData(-4, -400)]
        public void ObserverPatternInitTest(int value, int expected)
        {
            Observable source = new Observable();

            Observer observerA = new Observer();
            Observer observerB = new Observer();

            int multipliedValue = 0;

            // observerを登録する
            var disposableSubscriberA = source.Subscribe(observerA);
            var disposableSubscriberB = source.Subscribe(observerB);

            // メソッドをCallではなくSubscribe内に式を書く場合
            var disposableSubscriberC = source.Subscribe(n => multipliedValue = MultipliedBy100(n));
            //source.Subscribe(x => multipliedValue = x * 100);

            var message = $"Observer: {value}を受信しました";
            source.Execute(value);

            Assert.Equal(expected, multipliedValue);

            Assert.Equal(message, observerA.Message);
            Assert.Equal(message, observerB.Message);

            Assert.Equal(expected, multipliedValue);


            disposableSubscriberA.Dispose();
            disposableSubscriberB.Dispose();
            disposableSubscriberC.Dispose();
        }

    }
}
