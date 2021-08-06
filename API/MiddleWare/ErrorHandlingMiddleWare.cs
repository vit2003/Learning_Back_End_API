using Application.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.MiddleWare
{
    public class ErrorHandlingMiddleWare
    {
        public RequestDelegate Next { get; }
        public ILogger<ErrorHandlingMiddleWare> Logger { get; }
        public ErrorHandlingMiddleWare(RequestDelegate next, ILogger<ErrorHandlingMiddleWare> logger)
        {
            Next = next;
            Logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await Next(context);
            } 
            catch (Exception e)
            {
                await HandleExceptionAsync(context, e, Logger);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception e, ILogger<ErrorHandlingMiddleWare> logger)
        {
            object errors = null;

            switch (e)
            {
                case RestException re:
                    logger.LogError(e, "REST ERROR");
                    errors = re.Error;
                    context.Response.StatusCode = (int)re.Code;
                    break;
                case Exception ex:
                    logger.LogError(e, "SERVER ERROR");
                    errors = string.IsNullOrWhiteSpace(ex.Message) ? "Error" : e.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";
            if(errors != null)
            {
                var result = JsonSerializer.Serialize(new
                {
                    errors
                });

                await context.Response.WriteAsync(result);
            }
        }
    }
}
