using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Bugsnag.AspNet.Core
{
  /// <summary>
  /// The Bugsnag AspNetCore middleware.
  /// </summary>
  public class Middleware
  {
    public const string HttpContextItemsKey = "Bugsnag.Client";

    private readonly RequestDelegate _next;

    public Middleware(RequestDelegate requestDelegate)
    {
      _next = requestDelegate;
    }

    public async Task Invoke(HttpContext context, IClient client)
    {
      if (client.Configuration.AutoCaptureSessions)
      {
        client.SessionTracking.CreateSession();
      }

      context.Items[HttpContextItemsKey] = client;

      try
      {
        await _next(context);
      }
      catch (System.Exception exception)
      {
        client.AutoNotify(exception, context);
        throw;
      }
    }
  }
}
