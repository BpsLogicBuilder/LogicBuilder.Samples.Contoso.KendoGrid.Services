using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Contoso.BSL.AutoMapperProfiles;
using Contoso.Contexts;
using Contoso.Repositories;
using Contoso.Stores;
using LogicBuilder.EntityFrameworkCore.Mapping;
using LogicBuilder.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

#pragma warning disable IDE0130 //Microsoft recommended namespace for service registrations
namespace Microsoft.Extensions.DependencyInjection
#pragma warning restore IDE0130
{
    public static class BslServiceRegistrations
    {
        public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
        {
            return services
                .AddSingleton<AutoMapper.IConfigurationProvider>
                (
                    ConfigurationHelper.GetMapperConfiguration(cfg =>
                    {
                        cfg.AddExpressionMapping();

                        cfg.AddProfile<ExpressionOperatorsMappingProfile>();
                        cfg.AddProfile<ExpressionParameterToDescriptorMappingProfile>();
                        cfg.AddProfile<ExpansionParameterToDescriptorMappingProfile>();
                        cfg.AddProfile<ExpansionDescriptorToOperatorMappingProfile>();
                        cfg.AddProfile<SchoolProfile>();
                    })
                )
                .AddTransient<IMapper>(sp => new Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>(), sp.GetService));
        }

        public static IServiceCollection AddGridRequestServices(this IServiceCollection services)
        {
            return services
                .AddTransient<ISchoolStore, SchoolStore>()
                .AddTransient<IContextRepository, SchoolRepository>()
                .AddTransient<ISchoolRepository, SchoolRepository>();
        }

        public static IServiceCollection AddSqlServerDatabaseConfiguration(this IServiceCollection services, string connectionString)
        {
            return services.AddDbContext<SchoolContext>
            (
                options => options.UseSqlServer
                (
                    connectionString,
                    options => options.EnableRetryOnFailure()
                ),
                ServiceLifetime.Transient
            );
        }
    }
}
