using Microsoft.OpenApi.Models;
using Application.HttpApi.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Application.HttpApi.Host.Swagger;

public class DynamicSwaggerGenerator : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var (route, method, iface) in AppServiceMetadata.Discover())
        {
            var parameters = method.GetParameters();

            OpenApiRequestBody requestBody = null;

            if (parameters.Length == 1)
            {
                try
                {
                    var param = parameters[0];
                    var schema = context.SchemaGenerator.GenerateSchema(param.ParameterType, context.SchemaRepository);

                    requestBody = new OpenApiRequestBody
                    {
                        Content =
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = schema
                        }
                    }
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
            }

            var operation = new OpenApiOperation
            {
                Summary = method.Name,
                Tags = new List<OpenApiTag> { new() { Name = iface.Name } },
                RequestBody = requestBody,
                Responses = new OpenApiResponses
                {
                    ["200"] = new OpenApiResponse
                    {
                        Description = "Success",
                        Content = {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema { Type = "object" }
                        }
                    }
                    }
                }
            };

            swaggerDoc.Paths.Add(route, new OpenApiPathItem
            {
                Operations = { [OperationType.Post] = operation }
            });
        }
    }
}
