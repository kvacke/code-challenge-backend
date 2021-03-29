using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrafficUpdateSubscriptionSystem.Models;
using TrafficUpdateSubscriptionSystem.Repositories;

namespace TrafficUpdateSubscriptionSystem.Models
{
    public class SubscriptionContext : DbContext, ISubscriptionRepository
    {
        public SubscriptionContext(DbContextOptions<SubscriptionContext> options) : base(options)
        {

        }

        public DbSet<Subscription> Subscriptions { get; set; }

        public IQueryable<Subscription> GetActiveSubscriptionsByArea(string area)
        {
            return Subscriptions.Where(sub => sub.Area == area);
        }

        public IQueryable<Subscription> GetAllActiveSubscriptions()
        {
            DateTime CutoffDateTime = DateTime.Now.AddDays(-1);
            return Subscriptions.Where(sub => sub.CreatedAt >= CutoffDateTime);
        }

        public Subscription GetSubscriptionByIdentifier(string identifier)
        {
            Subscription sub = Subscriptions.Where(s => s.Identifier == identifier).SingleOrDefault();
            return sub;
        }

        public void RemoveSubscriptionByIdentifier(string identifier)
        {
            Subscriptions.Remove(GetSubscriptionByIdentifier(identifier));
        }

    }
}
