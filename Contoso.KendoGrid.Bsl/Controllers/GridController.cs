using Kendo.Mvc.UI;
using LogicBuilder.App.KendoGrid.Bsl.Business.Requests;
using LogicBuilder.App.KendoGrid.Bsl.Utils.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.KendoGrid.Bsl.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GridController(IRequestHelper requestHelper) : ControllerBase
    {
        private readonly IRequestHelper _requestHelper = requestHelper;

        [HttpPost("GetData")]
        public Task<DataSourceResult> GetData([FromBody] KendoGridDataRequest request) 
            => _requestHelper.GetData(request);
    }
}
