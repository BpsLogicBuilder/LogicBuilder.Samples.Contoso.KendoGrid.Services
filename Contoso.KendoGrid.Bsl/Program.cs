namespace Contoso.KendoGrid.Bsl
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Program 
    {
        protected Program() {}

        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services
                .AddSqlServerDatabaseConfiguration(builder.Configuration.GetConnectionString("DefaultConnection")!)
                .AddLogging()
                .AddAutoMapperConfiguration()
                .AddKendoGridBslUtilsServices()
                .AddAppUtilsMappingOperations()
                .AddGridRequestServices();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}