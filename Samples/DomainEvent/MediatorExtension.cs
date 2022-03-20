using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Samples.SeedWork;


namespace Samples.DomainEvent
{
    /// <summary>
    /// ドメインイベントの発行処理
    /// </summary>
    static class MediatorExtension
    {

        public static async Task EntityDispatchDomainEventAsync<T>(this IMediator mediator, T entity) where T : Entity
        {
            var domainEvents = entity.DomainEvents.ToList();

            entity.ClearDomainEvents();

            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }
    }
}
