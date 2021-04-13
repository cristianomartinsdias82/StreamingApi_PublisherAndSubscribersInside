using System;

namespace EventsPublishingApi.Core
{
    public class Customer : Entity<int>
    {
        public DateTime CustomerSince { get; set; }
        public Name Name { get; set; }
    }
}
