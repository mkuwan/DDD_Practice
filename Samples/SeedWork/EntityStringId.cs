using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Samples.SeedWork
{
    public abstract class EntityStringId
    {
        private int? _requestedHashCode;
        private string _Id;

        public virtual string Id
        {
            get => _Id;
            protected set => _Id = value;
        }

        private List<INotification> _domainEvents;
        public IReadOnlyCollection<INotification> DomainEvent => _domainEvents.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public bool IsTransient()
        { 
            return this.Id == default || this.Id == String.Empty;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj is EntityStringId) return false;

            if(Object.ReferenceEquals(this, obj)) return true;

            if(this.GetType() != obj.GetType()) return false;

            EntityStringId item = (EntityStringId)obj;

            if (item.IsTransient() == this.IsTransient())
                return false;
            else
                return item.Id.ToString() == this.Id.ToString();
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31;

                return _requestedHashCode.Value;
            }
            return base.GetHashCode();
        }

        public static bool operator ==(EntityStringId left, EntityStringId right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(EntityStringId left, EntityStringId right)
        {
            return !(left == right);
        }
    }
}
