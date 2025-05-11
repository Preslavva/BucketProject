using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;

public class MvcTryCatchFilter : IAsyncActionFilter
{
    private readonly IModelMetadataProvider _meta;
    private readonly ITempDataDictionaryFactory _temp;

    public MvcTryCatchFilter(
        IModelMetadataProvider meta,
        ITempDataDictionaryFactory temp)
    {
        _meta = meta;
        _temp = temp;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        try
        {
            // Run the action
            var resultContext = await next();

            // If the action threw but didn’t handle it, rethrow so we catch below
            if (resultContext.Exception != null && !resultContext.ExceptionHandled)
                throw resultContext.Exception;
        }
        catch (ArgumentException ex)
        {
            // re-render the same view with error in ViewData
            var action = context.ActionDescriptor.RouteValues["action"]!;
            var model = context.ActionArguments.Values.FirstOrDefault();

            var vd = new ViewDataDictionary(_meta, context.ModelState) { Model = model };
            vd["ErrorMessage"] = ex.Message;

            context.Result = new ViewResult
            {
                ViewName = action,
                ViewData = vd,
                TempData = _temp.GetTempData(context.HttpContext)
            };
        }
        catch
        {
            // other exceptions bubble through or you can handle generically
            throw;
        }
    }
}
