﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Payment.Acquiring;
using Payment.Acquiring.Configuration;
using Payment.Api.Configuration;
using System;

namespace Payment.Api
{
    public static class ThirdPartyHttpClient
    {
        /// <summary>
        /// Add http named client
        /// </summary>
        /// <param name="services"></param>
        /// <param name="clientName"></param>
        /// <param name="configSection"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpClient(this IServiceCollection services, string clientName, string configSection, IConfiguration configuration)
        {
            services.Configure<ThirdPartyModel>(configuration.GetSection(configSection));
            services.AddHttpClient(clientName)
                  .ConfigureHttpClient((sp, options) =>
                  {
                      var httpClientOptions = sp.GetRequiredService<IOptions<ThirdPartyModel>>().Value;
                      options.BaseAddress = httpClientOptions.BaseAddress;
                      options.Timeout = httpClientOptions.Timeout;
                  });

            return services;
        }

        /// <summary>
        /// Add acquiring banks
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAcquiringBank(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient(Constants.REAL_BANK, "ThirdPartySettings:Bank", configuration); ;

            return services;
        }

        /// <summary>
        /// Add Mock Bank
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAcquiringBankMock(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FeaturesModel>(configuration.GetSection("Features"));

            services.AddHttpClient(Constants.MOCK_BANK)
                  .ConfigurePrimaryHttpMessageHandler(() => new MockHttpMessageHandler())
                  .ConfigureHttpClient((sp, options) =>
                  {
                      options.BaseAddress = new Uri("http://mock-bank");
                  });

            return services;
        }
    }
}