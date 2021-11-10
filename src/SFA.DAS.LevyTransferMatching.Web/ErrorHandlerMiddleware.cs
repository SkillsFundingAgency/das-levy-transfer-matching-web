using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SFA.DAS.LevyTransferMatching.Infrastructure.Exceptions;

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
            var statusCode = -1;

            try
            {
                await _next(context);
            }
            catch (NullModelException)
            {
                statusCode = 404;
            }
            catch (Exception e)
            {
                statusCode = 500;

                _logger.LogError(e, "The request failed");
            }

            if (statusCode != -1)
            {
                context.Response.Redirect($"/Home/Error/{statusCode}");
            }
        }
    }
}
