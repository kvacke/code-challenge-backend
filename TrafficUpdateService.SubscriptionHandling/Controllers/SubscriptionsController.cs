using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TrafficUpdateSubscriptionSystem.Models;
using TrafficUpdateSubscriptionSystem.Validators;

namespace TrafficUpdateSubscriptionSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly SubscriptionContext _context;
        private readonly ILogger<SubscriptionsController> _logger;
        private readonly ITrafficDataAccess _dataAccess;

        public SubscriptionsController(ILogger<SubscriptionsController> logger, SubscriptionContext context, ITrafficDataAccess dataAccess)
        {
            _logger = logger;
            _context = context;
            _dataAccess = dataAccess;
        }

        // GET: api/Subscriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscription()
        {
            return await _context.Subscriptions.ToListAsync();
        }

        // GET: api/Subscriptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Subscription>> GetSubscription(string id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);

            if (subscription == null)
            {
                return NotFound();
            }

            return subscription;
        }


        // POST: api/Subscriptions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Subscription>> PostSubscription(Subscription subscription)
        {
            if (SubscriptionExists(subscription.Identifier))
            {
                _context.RemoveSubscriptionByIdentifier(subscription.Identifier);
            }

            subscription.Area = await _dataAccess.GetAreaFromGeolocation(subscription.Longitude, subscription.Latitude);

            _context.Subscriptions.Add(subscription);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SubscriptionExists(subscription.Identifier))
                {
                    return StatusCode(409);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSubscription", new { id = subscription.Identifier }, subscription);
        }

        //DELETE: api/Subscriptions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubscription(string id)
        {
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }

            _context.Subscriptions.Remove(subscription);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool SubscriptionExists(string id)
        {
            return _context.Subscriptions.Any(e => e.Identifier == id);
        }
    }
}
