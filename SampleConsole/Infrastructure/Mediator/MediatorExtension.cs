using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SampleConsole.Domain.SeedWork;


namespace SampleConsole.Infrastructure.Mediator
{
    /// <summary>
    /// ドメインイベントの発行処理
    /// </summary>
    static class MediatorExtension
    {
        public static async Task DbContextDispatchDomainEventAsync<T>(this IMediator mediator, T ctx) where T : DbContext
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null &&
                            x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList().ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }

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
