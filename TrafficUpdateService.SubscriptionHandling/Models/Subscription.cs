using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TrafficUpdateSubscriptionSystem.Models
{
    public class Subscription
    {
        [Key]
        public string Identifier { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Area { get; set; }
        public DateTime CreatedAt { get;  set; } = DateTime.Now;

    }
}
