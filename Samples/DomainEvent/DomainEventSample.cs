using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;


namespace Samples.DomainEvent
{
    // ドメインイベント
    // https://docs.microsoft.com/ja-jp/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation
    // https://enterprisecraftsmanship.com/posts/domain-events-simple-reliable-solution/
    // https://udidahan.com/2008/08/25/domain-events-take-2/

    /// <summary>
    /// マーカーインターフェース
    /// 使わないでと言われるけど...
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
        private static readonly List<Delegate> Handlers = new ();

        public static void Register<T>(Action<T> eventHandler) where T : IDomainEvent
        {
            Handlers.Add(eventHandler);
        }


        public static void Raise<T>(T domainEvent) where T : IDomainEvent
        {
            foreach (Delegate handler in Handlers)
            {
                var action = (Action<T>) handler;
                action(domainEvent);
            }
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
        public string _customer { get; private set; }
        public List<string> _goodsList { get; private set; }



        public Order(string customer)
        {
            _customer = customer;
            _goodsList = new List<string>();
        }

        public void AddCart(string item)
        {
            _goodsList.Add(item);
        }

        public Order Submitted()
        {
            DomainEvents.Raise(new OrderSubmittedEvent(this));
            return this;
        }
    }

    public class DomainEventSampleTest
    {
        private int actualValue = 0;

        [Fact]
        public void ClassicalDomainEventApproachTest()
        {
            // Arrange
            // イベント購読
            DomainEvents.Register<OrderSubmittedEvent>(eventHandler);
            Order Cart = new Order("客");
            var Goods = new List<string>() {"シャンプー", "リンス", "石鹸"};
            Goods.ForEach(x => Cart.AddCart(x));


            // Act
            Assert.Equal(0, actualValue); //呼び出し前は計算されていないので0
            Cart.Submitted(); // eventHandlerが呼び出される

            // Assertion
            Assert.Equal("客", Cart._customer);
            Assert.Equal(1500, actualValue);
        }

        private void eventHandler(OrderSubmittedEvent ev)
        {
            // to do...

            // ex)
            actualValue = ev.Order._goodsList.Count * 500;
        }
    }

}
