using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Tf1DayOff.Api.Constants;

namespace Tf1DayOff.Api.StartupHelpers;

public class AddRequiredHeaderParameter : IOperationFilter
{

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null)
            operation.Parameters = new List<OpenApiParameter>();

        operation.Parameters.Add(new  OpenApiParameter()
        {
            Name = ApiConstants.XUser,
            In = ParameterLocation.Header,
            Required = false,
            
        });
    }
}