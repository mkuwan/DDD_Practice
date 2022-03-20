﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.RabbitMQ
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);

        void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        void SubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler;

        void Unsubscribe<T, TH>() where T: IntegrationEvent where TH : IIntegrationEventHandler<T>;

        
        void UnsubscribeDynamic<TH>(string eventName) where TH : IDynamicIntegrationEventHandler;
    }
}