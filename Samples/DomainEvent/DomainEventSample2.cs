using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.DomainEvent
{
    /// <summary>
    /// https://tonytruong.net/domain-events-pattern-example/
    /// </summary>
    internal class DomainEventSample2
    {
        /// <summary>
        /// マーカーインターフェース
        /// </summary>
        public interface IDomainEvent { }

        /// <summary>
        /// IEventDispatcherは、実際にはNinjectやCastleWindsorなどのIoCコンテナになります
        /// </summary>
        public interface IEventDispatcher
        {
            void Dispatch<TEvent>(TEvent eventToDispatch) where TEvent : IDomainEvent;
        }

        /// <summary>
        /// DomainEventクラス
        /// </summary>
        public static class DomainEvents
        {
            public static IEventDispatcher Dispatcher { get; set; }

            public static void Raise<T>(T @event) where T : IDomainEvent
            {
                Dispatcher.Dispatch(@event);
            }
        }
    }
}
