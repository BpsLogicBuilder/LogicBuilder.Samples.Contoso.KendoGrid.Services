# LogicBuilder.Samples.Contoso.KendoGrid.Services

A comprehensive .NET sample application demonstrating how to build backend services for Kendo UI Grid components with support for advanced data operations including grouping, filtering, sorting, paging, and aggregation.

## Overview

This solution provides a complete, production-ready implementation showcasing:
- **Grouping** - Organize grid data hierarchically by one or more fields
- **Filtering** - Apply complex filter expressions to query data
- **Sorting** - Single and multi-column sorting capabilities
- **Paging** - Efficient server-side pagination
- **Aggregation** - Calculate sum, average, min, max, and count operations
- **Entity Framework Core** integration with SQL Server
- **AutoMapper** for mapping between domain entities and view models
- **Repository pattern** for data access
- **Dependency injection** with ASP.NET Core
- **Comprehensive test coverage** with xUnit and Testcontainers

## Solution Structure

The solution contains **10 projects** organized in a layered architecture:

### Domain Layer (.NET Standard 2.0)
- **Contoso.Domain** - Domain models and view models (StudentModel, InstructorModel, DepartmentModel, etc.) with LogicBuilder attributes for configuration
- **Contoso.Data** - Data transfer objects and entity definitions

### Data Access Layer (.NET 10)
- **Contoso.Contexts** - Entity Framework Core DbContext (SchoolContext) and SQL Server configuration
- **Contoso.Stores** - Data store abstractions implementing the Store pattern
- **Contoso.Repositories** - Repository implementations for data access (SchoolRepository)

### Business Logic Layer (.NET 10)
- **Contoso.BSL.AutoMapperProfiles** - AutoMapper profiles for mapping between entities and models, including:
  - SchoolProfile (Student ↔ StudentModel, Instructor ↔ InstructorModel, etc.)
  - Expression mapping profiles for LINQ expression translation
  - Expansion parameter mapping for OData-style query expansion

### Web/API Layer (.NET 10)
- **Contoso.KendoGrid.Bsl** - ASP.NET Core Web API project with:
  - `GridController` - REST API endpoint for Kendo Grid data requests
  - Service registration and dependency injection configuration
  - Docker support for containerized deployment

### Testing Layer (.NET 10)
- **Contoso.KendoGrid.Bsl.Tests** - Comprehensive xUnit test suite featuring:
  - Integration tests using Testcontainers for SQL Server
  - Tests for grouped/ungrouped data with various combinations of filters, sorts, and aggregates
  - Coverage for edge cases and error handling
  - Database seeding utilities for test data

## Key Components

### GridController
ASP.NET Core API controller that processes Kendo Grid data requests:

```c#
    [Route("api/[controller]")]
    [ApiController]
    public class GridController(IRequestHelper requestHelper) : ControllerBase
    {
        private readonly IRequestHelper _requestHelper = requestHelper;

        [HttpPost("GetData")]
        public Task<DataSourceResult> GetData([FromBody] KendoGridDataRequest request) 
            => _requestHelper.GetData(request);
    }
```

## Use Cases

This sample is ideal for developers who need to:
- Implement production-grade Kendo UI Grid backends in .NET
- Learn layered architecture patterns with EF Core
- Build type-safe, testable data access layers
- Understand AutoMapper expression mapping for dynamic queries
- Implement server-side grid operations efficiently
- Create containerized ASP.NET Core APIs


