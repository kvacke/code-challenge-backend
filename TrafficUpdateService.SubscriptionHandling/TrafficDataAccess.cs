using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using TrafficUpdateSubscriptionSystem.Models;

namespace TrafficUpdateSubscriptionSystem
{
    public class TrafficDataAccess : ITrafficDataAccess
    {
        private ILogger<TrafficDataAccess> _logger;
        private HttpClient _httpClient;

        public TrafficDataAccess(HttpClient httpClient, ILogger<TrafficDataAccess> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
            _logger.LogInformation("Creating data access object...");
        }

        public async Task<XElement> GetAllTrafficMessages()
        {
            var uri = "v2/traffic/messages?pagination=false";

            var responseString = await _httpClient.GetStringAsync(uri);

            XElement result = XElement.Parse(responseString).Element("messages");

            return result;
        }

        public async Task<string> GetAreaFromGeolocation(double longitude, double latitude)
        {
            var uri = $"v2/traffic/areas?latitude={latitude}&longitude={longitude}";

            var responseString = await _httpClient.GetStringAsync(uri);

            XElement result = XElement.Parse(responseString);

            string area = result.Descendants("area").First().Attribute("name").Value;

            return area;

        }

        private Notification CreateNotificationFromMessage(XElement message)
        {
            var title = message.Element("title").Value;
            var description = message.Element("description").Value;
            var priority = message.Attribute("priority").Value;
            var id = message.Attribute("id").Value;
            var category = message.Element("category").Value;

            Notification notification = new Notification(id, title, description, priority, category);

            return notification;
        }
    }
}
