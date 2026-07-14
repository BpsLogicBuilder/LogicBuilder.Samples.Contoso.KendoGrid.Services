using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.KendoGrid.Bsl.Controllers
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [ApiController]
#pragma warning disable S6931//all routes use full path
    public class ErrorController : ControllerBase
#pragma warning restore S6931
    {
        [Route("/error-local-development")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ErrorLocalDevelopment(
            [FromServices] IWebHostEnvironment webHostEnvironment)
        {
            if (webHostEnvironment.EnvironmentName != "Development")
            {
                throw new InvalidOperationException(
                    "This shouldn't be invoked in non-development environments.");
            }

            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

            return Ok
            (
                new
                {
                    Success = false,
                    ErrorMessages = new List<string>
                    { 
                        context?.Error.Message ?? "",
                        context?.Error.StackTrace ?? ""
                    }
                }
            );
        }

        [Route("/error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error() => Problem();
    }
}
