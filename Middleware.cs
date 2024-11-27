using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Bugsnag.AspNet.Core;

/// <summary>
/// The Bugsnag AspNetCore middleware.
/// </summary>
public class Middleware(RequestDelegate requestDelegate)
{
    public const string HttpContextItemsKey = "Bugsnag.Client";

    public async Task Invoke(HttpContext context, IClient client)
    {
        if (client.Configuration.AutoCaptureSessions)
        {
            client.SessionTracking.CreateSession();
        }

        // capture the request information now as the http context
        // may be changed by other error handlers after an exception
        // has occurred
        var bugsnagRequestInformation = context.ToRequest();

        client.BeforeNotify(report =>
        {
            report.Event.Request = bugsnagRequestInformation;
        });

        context.Items[HttpContextItemsKey] = client;

        if (client.Configuration.AutoNotify)
        {
            try
            {
                await requestDelegate(context);
            }
            catch (System.Exception exception)
            {
                client.Notify(exception, Payload.HandledState.ForUnhandledException());
                throw;
            }
        }
        else
        {
            await requestDelegate(context);
        }
    }
}