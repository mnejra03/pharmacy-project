using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NewPharmacy.Helper.Auth
{
    public class MyAuthorizationSwaggerHeader : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Description = "Unesite Bearer token za autorizaciju",
                Required = false, // Ako želite obavezno dodavanje tokena, postavite na true
                Schema = new OpenApiSchema { Type = "string" }
            });
        }
    }
}
