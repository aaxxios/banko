using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Logger.Filters
{
    public class RequestIpAsyncFilter : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();
            //if(ip == null || ip.StartsWith("197.210.227") || ip.StartsWith("::1"))
            //{
            //    context.Result = new ContentResult()
            //    {
            //        Content = "<h1>Unavailable at the moment", ContentType = "text/html"
            //    };
            //    return;
            //}
            await next();
        }
    }

   
}
