using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using Moq;
using Xunit;
using IContainer = Autofac.IContainer;


namespace Samples.DomainEvent
{
    // ドメインイベント
    // https://docs.microsoft.com/ja-jp/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation
    // https://enterprisecraftsmanship.com/posts/domain-events-simple-reliable-solution/
    // https://udidahan.com/2008/08/25/domain-events-take-2/
    // https://udidahan.com/2009/06/14/domain-events-salvation/

    /// <summary>
    /// マーカーインターフェース
    /// </summary>
    public interface IDomainEvent { }


    /// <summary>
    /// イベントを購読して、ドメインイベントを発生させる静的クラス
    /// ドメイン駆動設計に固有のドメインイベントの概念を最初に導入したのはUdiDahanだったと思います。
    /// 考え方は単純です。ドメインにとって重要なイベントを示したい場合は、
    /// このイベントを明示的に発生させ、ドメインモデル内の他のクラスにサブスクライブさせてそれに反応させます。
    /// 
    /// Udi Dahan がもともと提案しているのは (たとえば、「Domain Events – Take 2」
    /// (ドメイン イベント – テイク 2 などの複数の関連する投稿を参照))、
    /// イベントの管理と生成に静的クラスを使う方法です。
    /// これには、DomainEvents.Raise(Event myEvent) のような構文を使用し、
    /// 呼び出されるとすぐにドメイン イベントを生成する DomainEvents という名前の静的クラスが含まれる場合があります。
    /// Jimmy Bogard も、ブログ投稿「Strengthening your domain:Domain Events」
    /// (ドメインの強化: ドメイン イベント) で同様のアプローチを推奨しています。
    ///
    /// ドメイン イベント クラスが静的である場合は、ハンドラーにもすぐにディスパッチします
    /// </summary>
    public static class DomainEvents
    {
        private static List<Delegate>? _actions;
        
        public static IContainer Container { get; set; }

        /// <summary>
        /// Domain Event登録(Subscribe)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventHandler"></param>
        public static void Register<T>(Action<T> eventHandler) where T : IDomainEvent
        {
            _actions ??= new List<Delegate>();
            _actions.Add(eventHandler);
        }

        /// <summary>
        /// Domain Event発行(Publish)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domainEvent"></param>
        public static void Raise<T>(T domainEvent) where T : IDomainEvent
        {
            var handler = Container.Resolve<IHandlers<T>>();
            handler.Handle(domainEvent);

            foreach (Delegate action in _actions)
            {
                if (action is Action<T> act)
                {
                    //act(domainEvent);
                    act.Invoke(domainEvent);
                }
            }
        }

        public static void ClearEvents()
        {
            _actions.Clear();
        }
    }

    /// <summary>
    /// 注文ドメインイベント
    /// </summary>
    public class OrderSubmittedEvent : IDomainEvent
    {
        public Order Order { get; }

        public OrderSubmittedEvent(Order order)
        {
            Order = order;
        }
    }

    /// <summary>
    /// 注文ドメインモデル
    /// </summary>
    public class Order
    {
        public string Customer { get; private set; }
        public List<(string, decimal)> GoodsList { get; private set; }

        public decimal Price { get; private set; } = 0;


        public Order(string customer)
        {
            Customer = customer;
            GoodsList = new List<(string, decimal)>();
        }

        public void AddCart((string, decimal) item)
        {
            GoodsList.Add(item);
        }

        public Order Submitted()
        {
            // 合計額を計算します
            GoodsList.ForEach(x => Price += x.Item2);
            
            DomainEvents.Raise(new OrderSubmittedEvent(this));
            return this;
        }
    }

    /// <summary>
    /// イベントハンドラー
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHandlers<in T> where T : IDomainEvent
    {
        void Handle(T domainEvent);
    }
    public class OrderSubmittedHandler : IHandlers<OrderSubmittedEvent>
    {
        public decimal Price { get; private set; }

        public void Handle(OrderSubmittedEvent args)
        {
            Price = args.Order.Price;
        }
    }

    public class DomainEventSampleTest
    {
        [Fact]
        public void DomainEventHandlerTest()
        {
            var handler = new OrderSubmittedHandler();
            using (var mock = AutoMock.GetLoose(cfg => cfg.RegisterInstance(handler).As<IHandlers<OrderSubmittedEvent>>()))
            {
                // Arrange
                // イベント購読
                mock.Create<IHandlers<OrderSubmittedEvent>>();
                DomainEvents.Container = mock.Container;

                var order = new Order("お客さん");
                DomainEvents.Register<OrderSubmittedEvent>(ev => order = ev.Order);
                var products = new List<(string, decimal)>() { ("シャンプー", 400), ("リンス", 380), ("石鹸", 210) };
                products.ForEach(x => order.AddCart(x));

                // Act
                order.Submitted(); // eventHandlerが呼び出される

                // Assertion
                Assert.Equal("お客さん", order.Customer);
                Assert.Equal(990, order.Price);

                Assert.Equal(990, handler.Price);
            }
        }

    }


}
