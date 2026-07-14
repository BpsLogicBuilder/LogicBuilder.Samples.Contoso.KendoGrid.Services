using Contoso.Contexts;
using Contoso.Data.Entities;
using Contoso.Domain.Entities;
using Contoso.KendoGrid.Bsl.Controllers;
using Kendo.Mvc.Infrastructure;
using Kendo.Mvc.UI;
using LogicBuilder.App.KendoGrid.Bsl.Business.Requests;
using LogicBuilder.App.KendoGrid.Bsl.Utils.Interfaces;
using LogicBuilder.EntityFrameworkCore.Repositories;
using LogicBuilder.Expressions.Utils.ExpansionDescriptors;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Contoso.KendoGrid.Bsl.Tests
{
    public class GridControllerTest : IClassFixture<DatabaseFixture>
    {
        public GridControllerTest(DatabaseFixture databaseFixture)
        {
            this.databaseFixture = databaseFixture;
            Initialize();
        }

        #region Fields
        private readonly DatabaseFixture databaseFixture;
        private static IServiceProvider? serviceProvider;
        #endregion Fields

        [Fact]
        public async Task Get_students_ungrouped_with_options_null()
        {
            //arrange
            KendoGridDataRequest request = new()
            {
                ModelType = typeof(StudentModel).AssemblyQualifiedName,
                DataType = typeof(Student).AssemblyQualifiedName,
            };
            IRequestHelper helper = serviceProvider!.GetRequiredService<IRequestHelper>();
            GridController controller = new(helper);

            //act
            DataSourceResult result = await controller.GetData(request);

            //assert
            Assert.Equal(11, result.Total);
            Assert.Equal(11, ((IEnumerable<StudentModel>)result.Data).Count());
            Assert.Null(result.AggregateResults);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("InvalidType")]
        public async Task Get_data_throws_invalidOperationException_for_invalid_modelType(string? modelType)
        {
            //arrange
            KendoGridDataRequest request = new()
            {
                ModelType = modelType,
                DataType = typeof(Student).AssemblyQualifiedName,
            };
            IRequestHelper helper = serviceProvider!.GetRequiredService<IRequestHelper>();
            GridController controller = new(helper);

            //act & assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await controller.GetData(request)
            );

            Assert.Contains("Model type", exception.Message);
            Assert.Contains("is invalid", exception.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("InvalidType")]
        public async Task Get_data_throws_invalidOperationException_for_invalid_dataType(string? dataType)
        {
            //arrange
            KendoGridDataRequest request = new()
            {
                ModelType = typeof(StudentModel).AssemblyQualifiedName,
                DataType = dataType,
            };
            IRequestHelper helper = serviceProvider!.GetRequiredService<IRequestHelper>();
            GridController controller = new(helper);

            //act & assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await controller.GetData(request)
            );

            Assert.Contains("Data type", exception.Message);
            Assert.Contains("is invalid", exception.Message);
        }

        [Fact]
        public async Task Get_students_ungrouped_with_aggregates_and_filter()
        {
            //arrange
            KendoGridDataRequest request = new()
            {
                Options = new KendoGridDataSourceRequestOptions
                {
                    Aggregate = "lastName-count~enrollmentDate-min",
                    Filter = "lastName~eq~'Spratt'",
                    Group = null,
                    Page = 1,
                    Sort = "enrollmentDate-asc",
                    PageSize = 5
                },
                ModelType = typeof(StudentModel).AssemblyQualifiedName,
                DataType = typeof(Student).AssemblyQualifiedName,
            };
            IRequestHelper helper = serviceProvider!.GetRequiredService<IRequestHelper>();
            GridController controller = new(helper);

            //act
            DataSourceResult result = await controller.GetData(request);

            //assert
            Assert.Equal(3, result.Total);
            Assert.Equal(3, ((IEnumerable<StudentModel>)result.Data).Count());
            Assert.Equal("Spratt", ((IEnumerable<StudentModel>)result.Data).First().LastName);
        }

        [Fact]
        public async Task Get_students_ungrouped_with_aggregates()
        {
            //arrange
            KendoGridDataRequest request = new()
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
                DataType = typeof(Student).AssemblyQualifiedName,
            };
            IRequestHelper helper = serviceProvider!.GetRequiredService<IRequestHelper>();
            GridController controller = new(helper);

            //act
            DataSourceResult result = await controller.GetData(request);

            //assert
            Assert.Equal(11, result.Total);
            Assert.Equal(5, ((IEnumerable<StudentModel>)result.Data).Count());
            Assert.Equal("Nino", ((IEnumerable<StudentModel>)result.Data).First().FirstName);
            Assert.Equal(2, result.AggregateResults.Count());
            Assert.Equal("Count", result.AggregateResults.First().AggregateMethodName);
            Assert.Equal(11, (int)result.AggregateResults.First().Value);
        }

        [Fact]
        public async Task Get_students_grouped_with_aggregates()
        {
            //arrange
            KendoGridDataRequest request = new()
            {
                Options = new KendoGridDataSourceRequestOptions
                {
                    Aggregate = "lastName-count~enrollmentDate-min",
                    Filter = null,
                    Group = "enrollmentDate-asc",
                    Page = 1,
                    Sort = null,
                    PageSize = 5
                },
                ModelType = typeof(StudentModel).AssemblyQualifiedName,
                DataType = typeof(Student).AssemblyQualifiedName,
            };
            IRequestHelper helper = serviceProvider!.GetRequiredService<IRequestHelper>();
            GridController controller = new(helper);

            //act
            DataSourceResult result = await controller.GetData(request);

            //assert
            Assert.Equal(11, result.Total);
            Assert.Equal(3, ((IEnumerable<AggregateFunctionsGroup>)result.Data).Count());
            Assert.Equal(2, result.AggregateResults.Count());
            Assert.Equal("Count", result.AggregateResults.First().AggregateMethodName);
            Assert.Equal(11, (int)result.AggregateResults.First().Value);
        }

        [Fact]
        public async Task Get_departments_ungrouped_with_aggregates_and_includes()
        {
            //arrange
            KendoGridDataRequest request = new()
            {
                Options = new KendoGridDataSourceRequestOptions
                {
                    Aggregate = "administratorName-min~name-count~budget-sum~budget-min~startDate-min",
                    Filter = null,
                    Group = null,
                    Page = 1,
                    Sort = null,
                    PageSize = 5
                },
                ModelType = typeof(DepartmentModel).AssemblyQualifiedName,
                DataType = typeof(Department).AssemblyQualifiedName,
            };
            IRequestHelper helper = serviceProvider!.GetRequiredService<IRequestHelper>();
            GridController controller = new(helper);

            //act
            DataSourceResult result = await controller.GetData(request);

            //assert
            Assert.Equal(4, result.Total);
            Assert.Equal(4, ((IEnumerable<DepartmentModel>)result.Data).Count());
            Assert.Equal(5, result.AggregateResults.Count());
            Assert.Equal("Kim Abercrombie", ((IEnumerable<DepartmentModel>)result.Data).First().AdministratorName);
            Assert.Equal("Min", result.AggregateResults.First().AggregateMethodName);
            Assert.Equal("Candace Kapoor", (string)result.AggregateResults.First().Value);
        }

        [Fact]
        public async Task Get_departments_grouped_with_aggregates()
        {
            //arrange
            KendoGridDataRequest request = new()
            {
                Options = new KendoGridDataSourceRequestOptions
                {
                    Aggregate = "administratorName-min~name-count~budget-sum~budget-min~startDate-min",
                    Filter = null,
                    Group = "budget-asc",
                    Page = 1,
                    Sort = null,
                    PageSize = 5
                },
                ModelType = typeof(DepartmentModel).AssemblyQualifiedName,
                DataType = typeof(Department).AssemblyQualifiedName,
            };
            IRequestHelper helper = serviceProvider!.GetRequiredService<IRequestHelper>();
            GridController controller = new(helper);

            //act
            DataSourceResult result = await controller.GetData(request);

            //assert
            Assert.Equal(4, result.Total);
            Assert.Equal(2, ((IEnumerable<AggregateFunctionsGroup>)result.Data).Count());
            Assert.Equal(5, result.AggregateResults.Count());
            Assert.Equal("Min", result.AggregateResults.First().AggregateMethodName);
            Assert.Equal("Candace Kapoor", (string)result.AggregateResults.First().Value);
        }

        [Fact]
        public async Task Get_instructors_ungrouped_with_aggregates()
        {
            //arrange
            KendoGridDataRequest request = new()
            {
                Options = new KendoGridDataSourceRequestOptions
                {
                    Aggregate = "lastName-count~hireDate-min",
                    Filter = null,
                    Group = null,
                    Page = 1,
                    Sort = null,
                    PageSize = 5
                },
                ModelType = typeof(InstructorModel).AssemblyQualifiedName,
                DataType = typeof(Instructor).AssemblyQualifiedName,
            };
            IRequestHelper helper = serviceProvider!.GetRequiredService<IRequestHelper>();
            GridController controller = new(helper);

            //act
            DataSourceResult result = await controller.GetData(request);

            //assert
            Assert.Equal(5, result.Total);
            Assert.Equal(5, ((IEnumerable<InstructorModel>)result.Data).Count());
            Assert.Equal(2, result.AggregateResults.Count());
            Assert.Equal("Roger Zheng", ((IEnumerable<InstructorModel>)result.Data).First().FullName);
        }

        [Fact]
        public async Task Get_instructors_grouped_with_aggregates_and_expansions()
        {
            //arrange
            KendoGridDataRequest request = new()
            {
                Options = new KendoGridDataSourceRequestOptions
                {
                    Aggregate = "lastName-count~hireDate-min",
                    Filter = null,
                    Group = "hireDate-asc",
                    Page = 1,
                    Sort = null,
                    PageSize = 5
                },
                ModelType = typeof(InstructorModel).AssemblyQualifiedName,
                DataType = typeof(Instructor).AssemblyQualifiedName,
                SelectExpandDefinition = new SelectExpandDefinitionDescriptor
                (
                    expandedItems:
                    [
                        new SelectExpandItemDescriptor("courses"),
                        new SelectExpandItemDescriptor("officeAssignment")
                    ]
                )
            };
            IRequestHelper helper = serviceProvider!.GetRequiredService<IRequestHelper>();
            GridController controller = new(helper);

            //act
            DataSourceResult result = await controller.GetData(request);

            //assert
            Assert.Equal(5, result.Total);
            Assert.Equal(5, ((IEnumerable<AggregateFunctionsGroup>)result.Data).Count());
            Assert.NotEmpty(((IEnumerable<AggregateFunctionsGroup>)result.Data).First().Items.Cast<InstructorModel>().First().Courses!);
            Assert.Equal(2, result.AggregateResults.Count());
            Assert.Equal("Count", result.AggregateResults.First().AggregateMethodName);
            Assert.Equal(5, (int)result.AggregateResults.First().Value);
        }

        [Fact]
        public async Task Get_instructors_grouped_with_aggregates_without_expansions()
        {
            //arrange
            KendoGridDataRequest request = new()
            {
                Options = new KendoGridDataSourceRequestOptions
                {
                    Aggregate = "lastName-count~hireDate-min",
                    Filter = null,
                    Group = "hireDate-asc",
                    Page = 1,
                    Sort = null,
                    PageSize = 5
                },
                ModelType = typeof(InstructorModel).AssemblyQualifiedName,
                DataType = typeof(Instructor).AssemblyQualifiedName
            };
            IRequestHelper helper = serviceProvider!.GetRequiredService<IRequestHelper>();
            GridController controller = new(helper);

            //act
            DataSourceResult result = await controller.GetData(request);

            //assert
            Assert.Equal(5, result.Total);
            Assert.Equal(5, ((IEnumerable<AggregateFunctionsGroup>)result.Data).Count());
            Assert.Empty(((IEnumerable<AggregateFunctionsGroup>)result.Data).First().Items.Cast<InstructorModel>().First().Courses);
            Assert.Null(((IEnumerable<AggregateFunctionsGroup>)result.Data).First().Items.Cast<InstructorModel>().First().OfficeAssignment);
            Assert.Equal(2, result.AggregateResults.Count());
            Assert.Equal("Count", result.AggregateResults.First().AggregateMethodName);
            Assert.Equal(5, (int)result.AggregateResults.First().Value);
        }

        [Fact]
        public async Task Get_instructors_grouped_with_no_aggregates_returns_no_aggregates()
        {
            //arrange
            KendoGridDataRequest request = new()
            {
                Options = new KendoGridDataSourceRequestOptions
                {
                    Aggregate = null,
                    Filter = null,
                    Group = "hireDate-asc",
                    Page = 1,
                    Sort = null,
                    PageSize = 5
                },
                ModelType = typeof(InstructorModel).AssemblyQualifiedName,
                DataType = typeof(Instructor).AssemblyQualifiedName
            };
            IRequestHelper helper = serviceProvider!.GetRequiredService<IRequestHelper>();
            GridController controller = new(helper);

            //act
            DataSourceResult result = await controller.GetData(request);

            //assert
            Assert.Equal(5, result.Total);
            Assert.Equal(5, ((IEnumerable<AggregateFunctionsGroup>)result.Data).Count());
            Assert.Empty(((IEnumerable<AggregateFunctionsGroup>)result.Data).First().Items.Cast<InstructorModel>().First().Courses);
            Assert.Null(((IEnumerable<AggregateFunctionsGroup>)result.Data).First().Items.Cast<InstructorModel>().First().OfficeAssignment);
            Assert.Null(result.AggregateResults);
        }

        #region Helpers
        [MemberNotNull(nameof(serviceProvider))]
        private void Initialize()
        {
            serviceProvider ??= new ServiceCollection()
                .AddSqlServerDatabaseConfiguration(databaseFixture.GetConnectionString(GetType().Name))
                .AddLogging()
                .AddAutoMapperConfiguration()
                .AddKendoGridBslUtilsServices()
                .AddAppUtilsMappingOperations()
                .AddGridRequestServices()
                .BuildServiceProvider();

            ReCreateDataBase(serviceProvider.GetRequiredService<SchoolContext>()).GetAwaiter().GetResult();
            DatabaseSeeder.Seed_Database(serviceProvider.GetRequiredService<IContextRepository>()).GetAwaiter().GetResult();
        }

        private static async Task ReCreateDataBase(SchoolContext context)
        {
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }
        #endregion Helpers
    }
}
