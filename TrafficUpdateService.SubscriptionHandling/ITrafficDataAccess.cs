using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TrafficUpdateSubscriptionSystem
{
    public interface ITrafficDataAccess
    {
        Task<XElement> GetAllTrafficMessages();
        Task<string> GetAreaFromGeolocation(double longitude, double latitude);
    }
}
