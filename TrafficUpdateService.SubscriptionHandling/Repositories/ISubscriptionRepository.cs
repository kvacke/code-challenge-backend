using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrafficUpdateSubscriptionSystem.Models;

namespace TrafficUpdateSubscriptionSystem.Repositories
{
    public interface ISubscriptionRepository
    {

        IQueryable<Subscription> GetAllActiveSubscriptions();
        Subscription GetSubscriptionByIdentifier(string identifier);
        void RemoveSubscriptionByIdentifier(string identifier);
        IQueryable<Subscription> GetActiveSubscriptionsByArea(string area);

    }
}
