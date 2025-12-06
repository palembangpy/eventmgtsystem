namespace EventManagementSystem.Security;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

public class SanitizeModelFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        foreach (var arg in context.ActionArguments.Values)
        {
            if (arg == null) continue;

            var properties = arg.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                if (prop.PropertyType != typeof(string)) continue;

                var sanitizeAttr = prop.GetCustomAttribute<SanitizeAttribute>();
                if (sanitizeAttr == null) continue;

                var value = prop.GetValue(arg) as string;
                if (value == null) continue;

                var clean = SanitizerEngine.SanitizeInput(
                    value,
                    logicalMaxLength: sanitizeAttr.LogicalMaxLength
                );

                if (clean == null)
                {
                    context.Result = new UnauthorizedObjectResult(new
                    {
                        error = "SECURITY_VIOLATION",
                        field = prop.Name,
                        message = "Dangerous input detected"
                    });

                    return;
                }

                prop.SetValue(arg, clean);
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
