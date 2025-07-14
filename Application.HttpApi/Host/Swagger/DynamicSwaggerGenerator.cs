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
            var param = method.GetParameters().FirstOrDefault();
            if (param == null)
                continue;

            OpenApiSchema schema;
            try
            {
                schema = context.SchemaGenerator.GenerateSchema(param.ParameterType, context.SchemaRepository);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                continue;
            }

            var operation = new OpenApiOperation
            {
                Summary = method.Name,
                Tags = new List<OpenApiTag> { new() { Name = iface.Name } },
                RequestBody = new OpenApiRequestBody
                {
                    Content =
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = schema
                        }
                    }
                },
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
