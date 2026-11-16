using Contoso.Domain.Entities;
using LogicBuilder.App.KendoGrid.Bsl.Business.Requests;
using LogicBuilder.App.Utils.Json;
using LogicBuilder.Expressions.Utils.Json;
using System.Text.Json;

namespace Contoso.KendoGrid.Api
{
    public static class SerializationOptions
    {
        private static JsonSerializerOptions? _default;
        public static JsonSerializerOptions Default
        {
            get
            {
                if (_default != null)
                    return _default;

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                options.Converters.Add(new DescriptorConverter());
                options.Converters.Add(new ObjectConverter(typeof(WeatherForecast).Assembly, typeof(DescriptorConverter).Assembly, typeof(StudentModel).Assembly, typeof(KendoGridDataRequest).Assembly));

                _default = options;

                return _default;
            }
        }
    }
}
