using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using TrafficUpdateSubscriptionSystem.Models;


namespace TrafficUpdateSubscriptionSystem.Validators
{
    public class SubscriptionValidator : AbstractValidator<Subscription>
    {
        public SubscriptionValidator()
        {
            RuleFor(subscription => subscription.Longitude).NotEmpty();
            RuleFor(subscription => subscription.Latitude).NotEmpty();
            RuleFor(subscription => subscription.Identifier).NotEmpty();
        }
    }
}
