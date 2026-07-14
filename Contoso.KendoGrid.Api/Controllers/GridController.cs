using LogicBuilder.App.KendoGrid.Bsl.Business.Requests;
using LogicBuilder.App.Utils.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Contoso.KendoGrid.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GridController(IHttpClientHelper httpClientHelper, IOptions<UrlOptions> optionsAccessor) : ControllerBase
    {
        private readonly IHttpClientHelper _httpClientHelper = httpClientHelper;
        private readonly UrlOptions urlOptions = optionsAccessor.Value;

        [HttpPost("GetData")]
        public Task<JsonObject> GetData([FromBody] KendoGridDataRequest request)
        {
            return _httpClientHelper.PostAsync<JsonObject>
            (
                $"{urlOptions.BaseBslUrl}api/Grid/GetData",
                JsonSerializer.Serialize(request),
                SerializationOptions.Default
            );
        }
    }
}
