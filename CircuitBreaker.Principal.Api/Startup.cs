using CircuitBreaker.Principal.Api.HttpFactories;
using CircuitBreaker.Principal.Api.HttpFactories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;

namespace CircuitBreaker.Principal.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            //var policyClass = new CircuitBreakerPatt();
            services.AddHttpClient("PaymentApi", client =>
            {
                client.BaseAddress = new Uri("https://localhost:5003");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.Timeout = TimeSpan.FromSeconds(20);
            });
            //.AddPolicyHandler(policyClass.GetRetryPolicy());
            //.AddPolicyHandler(policyClass.GetCircuitBreakerPolicy());

            services.AddScoped<IPaymentHttpFactory, PaymentHttpFactory>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Log.Information($"Hosting enviroment = {env.EnvironmentName}");

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
