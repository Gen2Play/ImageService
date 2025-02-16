using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shared.Middlewares;

public class GlobalException(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        string message = "Sorry, Some internal exception occured. Try again later";
        int status = (int)HttpStatusCode.InternalServerError;
        string title = "Error";

        try
        {
            await next(context);
            // check if Exception is too many request (429)
            if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
            {
                title = "Warning";
                message = "Too Many Request";
                status = (int)HttpStatusCode.TooManyRequests;
                await ModifyHeader(context, title, message, status);
            }

            //check if Unauthorize (401)
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                title = "Alert";
                message = "Unauthorized";
                status = (int)HttpStatusCode.Unauthorized;
                await ModifyHeader(context, title, message, status);
            }
            // check if response is Forbidden (403)
            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                title = "Out of Access";
                message = "You are not allow to access";
                status = (int)HttpStatusCode.Forbidden;
                await ModifyHeader(context, title, message, status);
            }
        }
        catch (Exception ex)
        {

            LogException.LogExceptions(ex, ex.Message);

            //check if Exception is Time out
            if (ex is TaskCanceledException || ex is TimeoutException)
            {
                message = "Request Time Out";
                title = "Out if time";
                status = StatusCodes.Status408RequestTimeout;
            }

            await ModifyHeader(context, title, message, status);
        }
    }

    private async Task ModifyHeader(HttpContext context, string title, string message, int status)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = status;
        await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails()
        {
            Detail = message,
            Status = status,
            Title = title,
        }), CancellationToken.None);
    }
}
