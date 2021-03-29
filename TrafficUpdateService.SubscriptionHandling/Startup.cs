using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrafficUpdateSubscriptionSystem.Models;
using TrafficUpdateSubscriptionSystem.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace TrafficUpdateSubscriptionSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SubscriptionContext>(opt => opt.UseInMemoryDatabase("CurrentSubscriptions"));
            services.AddControllers()
                        .AddFluentValidation(s =>
                        {
                            s.RegisterValidatorsFromAssemblyContaining<Startup>();
                            s.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                        });
            services.AddHttpClient<ITrafficDataAccess, TrafficDataAccess>(client =>
            {
                client.BaseAddress = new Uri(Configuration["BaseUrl"]);
            });
            services.AddHostedService<NotificationService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
