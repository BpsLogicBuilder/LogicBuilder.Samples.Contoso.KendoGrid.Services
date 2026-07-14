using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace Contoso.KendoGrid.Bsl.Tests
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private MsSqlContainer? _msSqlContainer;

        public string GetConnectionString(string initialCatalog)
        {
            if (_msSqlContainer == null)
                throw new InvalidOperationException("Container is not initialized.");

            return new SqlConnectionStringBuilder(_msSqlContainer.GetConnectionString())
            {
                InitialCatalog = initialCatalog
            }.ToString();
        }

        async ValueTask IAsyncLifetime.InitializeAsync()
        {
            _msSqlContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
                                .Build();
            await _msSqlContainer.StartAsync();
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (_msSqlContainer != null)
                await _msSqlContainer.DisposeAsync();

            GC.SuppressFinalize(this);
        }
    }
}
