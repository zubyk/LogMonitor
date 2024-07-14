using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace LogMonitor
{
    public class OperationCancelledExceptionFilter : ExceptionFilterAttribute
    {
        public OperationCancelledExceptionFilter()
        {
        }

        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is OperationCanceledException)
            {
                context.ExceptionHandled = true;
                context.Result = new StatusCodeResult(499);
            }
        }
    }
}
