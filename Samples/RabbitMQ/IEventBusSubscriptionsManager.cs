using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.RabbitMQ
{
    public interface IEventBusSubscriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;
        void AddSubscription<T, TH>() where TH : IIntegrationEventHandler<T> where T : IntegrationEvent;
        void AddDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler;
        void RemoveSubscription<T,TH>() where T: IntegrationEvent where TH : IIntegrationEventHandler<T>;
        void RemoveDynamicSubscription<TH>(string eventName) where TH : IDynamicIntegrationEventHandler;
        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent;
        IEnumerable<SubscriptionInfo> GetHandlerForEvent(string eventName);
        string GetEventKey<T>();
    }
}
