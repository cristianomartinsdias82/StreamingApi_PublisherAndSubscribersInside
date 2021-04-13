using System;

namespace EventsPublishingApi.Core
{
    public abstract class Entity<TKey> where TKey : IConvertible
    {
        public TKey Id { get; set; }
    }
}
