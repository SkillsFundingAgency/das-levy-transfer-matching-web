using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.LevyTransferMatching.Web
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            bool hasErrored = false;
            
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                hasErrored = true;

                _logger.LogError(e, "The request failed");
            }

            if (hasErrored)
            {
                context.Response.Redirect("/Home/Error/500");
            }
        }
    }
}
