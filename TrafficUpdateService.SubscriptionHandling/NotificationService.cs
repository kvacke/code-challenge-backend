using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TrafficUpdateSubscriptionSystem.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace TrafficUpdateSubscriptionSystem
{
    public class NotificationService : BackgroundService
    {
        private ITrafficDataAccess _dataAccess;
        private ILogger<NotificationService> _logger;
        private XElement _currentTrafficMessages;
        private IServiceScopeFactory _scopeFactory;
        private List<Subscription> _activeSubscriptions;
        private Dictionary<string,string> _messageIdAndAreaCache;
        private Dictionary<string,string> _subscriptionIdAndAreaCache;
        private DateTime _nextReset;

        public NotificationService(ITrafficDataAccess dataAccess, ILogger<NotificationService> logger,
                                   IServiceScopeFactory scopeFactory)
        {
            _dataAccess = dataAccess;
            _logger = logger;
            _currentTrafficMessages = new XElement("empty");
            _scopeFactory = scopeFactory;
            _messageIdAndAreaCache = new Dictionary<string, string>();
            _subscriptionIdAndAreaCache = new Dictionary<string, string>();
            _nextReset = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(1).Day, 7, 0, 0);
        }

       

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested){
                _currentTrafficMessages = await _dataAccess.GetAllTrafficMessages();
                _activeSubscriptions = GetActiveSubscriptions();
                await LookForNewMessages();
                LookForNewSubscriptions();
                HandleCacheReset();

                await Task.Delay(10000, stoppingToken);
            }
        }

        private void HandleCacheReset()
        {
            if(DateTime.Now > _nextReset)
            {
                _messageIdAndAreaCache.Clear();
                _subscriptionIdAndAreaCache.Clear();
                _nextReset = _nextReset.AddDays(1);
            }
        }

        private void LookForNewSubscriptions()
        {
            foreach (var sub in _activeSubscriptions)
            {
                if(!_subscriptionIdAndAreaCache.ContainsKey(sub.Identifier))
                {
                    _logger.LogWarning("Found new subscription with identifier: " + sub.Identifier + " and area " + sub.Area);

                    HandleNewSubscriber(sub.Identifier,sub.Area);
                }
            }
        }

        private void HandleNewSubscriber(string identifier, string area)
        {
            _subscriptionIdAndAreaCache.Add(identifier, area);
            var relevantMessageIds = GetMessageIdsFromCacheByArea(area);
            var relevantMessages = GetMessagesByIds(relevantMessageIds);
            if(isEmailAddress(identifier))
            {
                foreach(var message in relevantMessages)
                {
                    SendEmailToSubscriber(identifier, CreateNotification(message));
                }
            }
            else
            {
                foreach (var message in relevantMessages)
                {
                    SendTextMessageToSubscriber(identifier, CreateNotification(message));
                }
            }
        }

        private List<XElement> GetMessagesByIds(List<string> messageIds)
        {
            var relevantMessages = _currentTrafficMessages.Descendants("message").Where(message => messageIds.Contains(message.Attribute("id").Value)).ToList();
            return relevantMessages;
        }

        private List<string> GetMessageIdsFromCacheByArea(string area)
        {
            var ls = _messageIdAndAreaCache.Where(messageData => messageData.Value == area).Select(messageData => messageData.Key).ToList();
            return ls;
        }

        private async Task LookForNewMessages()
        {
            string messageId;

            foreach (var message in _currentTrafficMessages.Elements()) 
            {
                messageId = message.Attribute("id").Value;
                if (!_messageIdAndAreaCache.ContainsKey(messageId))
                {
                    _logger.LogWarning("Found new message with id: " + messageId);
                    await HandleNewMessage(message, messageId);
                }
            }
        }

        private async Task HandleNewMessage(XElement message, string messageId)
        {
            string messageArea = await GetMessageAreaFromGeolocation(message);
            _messageIdAndAreaCache.Add(messageId, messageArea);
            SendNotificationToSubscribersInArea(message, messageArea);
        }

        private void SendNotificationToSubscribersInArea(XElement message, string messageArea)
        {
            var subscriptionsInArea = GetActiveSubscriptionIdentifiersFromCacheByArea(messageArea);
            foreach(string identifier in subscriptionsInArea)
            {
                if (isEmailAddress(identifier))
                {
                    SendEmailToSubscriber(identifier, CreateNotification(message));
                }
                else
                {
                    SendTextMessageToSubscriber(identifier, CreateNotification(message));
                }
            }
        }

        private List<string> GetActiveSubscriptionIdentifiersFromCacheByArea(string messageArea)
        {
            var ls = _subscriptionIdAndAreaCache.Where(sub => sub.Value == messageArea).Select(sub => sub.Key).ToList();
            return ls;
        }



        private List<Subscription> GetActiveSubscriptions()
        {
            using var scope = _scopeFactory.CreateScope();
            using var context = scope.ServiceProvider.GetService<SubscriptionContext>();
            return context.GetAllActiveSubscriptions().ToList();
        }


        private async Task<string> GetMessageAreaFromGeolocation(XElement message)
        {
            var messageLongitude = double.Parse(message.Element("longitude").Value);
            var messageLatitude = double.Parse(message.Element("latitude").Value);
            string area = await _dataAccess.GetAreaFromGeolocation(messageLongitude, messageLatitude);
            return area;
        }


        private void SendTextMessageToSubscriber(string subscriberPhoneNumber, Notification notification)
        {
            _logger.LogWarning($"Sending traffic update via SMS to {subscriberPhoneNumber}....");
            _logger.LogWarning(notification.ToString());
        }

        private void SendEmailToSubscriber(string subscriberEmail, Notification notification)
        {
            _logger.LogWarning($"Sending traffic update via email to {subscriberEmail}....");
            _logger.LogWarning(notification.ToString());
        }

        private Notification CreateNotification(XElement message)
        {
            var title = message.Element("title").Value;
            var description = message.Element("description").Value;
            var priority = message.Attribute("priority").Value;
            var id = message.Attribute("id").Value;
            var category = message.Element("category").Value;

            Notification notification = new Notification(id, title, description, priority, category);

            return notification;
        }

        private bool isEmailAddress(string identifier)
        {
            return identifier.Contains("@");
        }


    }
}
