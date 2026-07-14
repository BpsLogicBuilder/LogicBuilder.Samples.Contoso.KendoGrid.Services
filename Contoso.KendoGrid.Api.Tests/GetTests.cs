using Contoso.Data.Entities;
using Contoso.Domain.Entities;
using LogicBuilder.App.KendoGrid.Bsl.Business.Requests;
using LogicBuilder.App.Utils.Web.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Threading.Tasks;

namespace Contoso.KendoGrid.Api.Tests
{
    public class GetTests
    {
        public GetTests()
        {
            Initialize();
        }

        #region Fields
        private IServiceProvider serviceProvider;
        #endregion Fields

        #region Properties
        private string BaseUrl
        {
            get
            {
                IOptions<UrlOptions> options = serviceProvider.GetRequiredService<IOptions<UrlOptions>>();
                string url = options.Value.BaseBslUrl;
                return url.EndsWith('/') ? url : $"{url}/";
            }
        }
        #endregion Properties

        #region Helpers
        [MemberNotNull(nameof(serviceProvider))]
        private void Initialize()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            IServiceCollection services = new ServiceCollection();
            services.AddAppUtilsHttpClientHelper();
            services.Configure<UrlOptions>(configuration);
            serviceProvider = services.BuildServiceProvider();
        }
        #endregion Helpers

        [Fact]
        public async Task Get_students_ungrouped_with_aggregates()
        {
            //arrange
            IHttpClientHelper httpClientHelper = serviceProvider.GetRequiredService<IHttpClientHelper>();
            var request = new KendoGridDataRequest
            {
                Options = new KendoGridDataSourceRequestOptions
                {
                    Aggregate = "lastName-count~enrollmentDate-min",
                    Filter = null,
                    Group = null,
                    Page = 1,
                    Sort = "enrollmentDate-asc",
                    PageSize = 5
                },
                ModelType = typeof(StudentModel).AssemblyQualifiedName,
                DataType = typeof(Student).AssemblyQualifiedName
            };

            //act//DataSourceResult
            var result = await httpClientHelper.PostAsync<System.Text.Json.Nodes.JsonObject>
            (
                $"{BaseUrl}api/Grid/GetData",
                JsonSerializer.Serialize
                (
                    request
                )
            );

            Assert.NotEmpty(result);
        }
    }
}
