using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Bugsnag.AspNet.Core;

public static class Extensions
{
    /// <summary>
    /// Add Bugsnag to your application. Configures the required bugsnag
    /// services and attaches the Bugsnag middleware to catch unhandled
    /// exceptions.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddBugsnag(this IServiceCollection services)
    {
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        return services
            .AddSingleton<IStartupFilter, BugsnagStartupFilter>()
            .AddScoped<IClient, Client>(context => {
                var configuration = context.GetService<IOptions<Configuration>>();
                var client = new Client(configuration.Value);
                return client;
            });
    }


    public static IServiceCollection AddBugsnag(this IServiceCollection services, Action<Configuration> configuration)
    {
        return services
            .AddBugsnag()
            .Configure(configuration);
    }
}