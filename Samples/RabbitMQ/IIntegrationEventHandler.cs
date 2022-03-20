using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples.RabbitMQ
{
    public interface IIntegrationEventHandler<in TEvent> where TEvent: IntegrationEvent
    {
        Task Handle(TEvent @event);
    }
}
