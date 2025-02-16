using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Middlewares;

public class ListenOnlyApiGateway(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        //Extract specific header from request
        var signedHeader = context.Request.Headers["Api-Gateway"];

        //Null means, this request is not coming from api gateway

        if (signedHeader.FirstOrDefault() == null)
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsync("Service is not available");
            return;
        }
        await next(context);
    }
}
