using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using ScalarVersion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

// ✅ Configura API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}); // ✅ necessário para registrar apiVersion constraint

// ✅ Configura o explorador de versão
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// ✅ Configura Swagger com suporte a versionamento
builder.Services.AddSwaggerGen(opt =>
{
    //opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //{
    //    BearerFormat = "JWT",
    //    Description = "JWT Authorization header using the Bearer scheme.",
    //    Name = "Authorization",
    //    In = ParameterLocation.Header,
    //    Type = SecuritySchemeType.Http,
    //    Scheme = "Bearer"
    //});
    //opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Id = "Bearer",
    //                Type = ReferenceType.SecurityScheme
    //            }
    //        },
    //        Array.Empty<string>()
    //    }
    //});
});

var app = builder.Build();

// 🔧 Obter provedor de descrições de versão
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// ✅ Configura Swagger e Scalar para múltiplas versões
app.UseSwagger(opt =>
{
    opt.RouteTemplate = "openapi/{documentName}.json";
});

app.MapScalarApiReference("/documentation",opt =>
{
    opt.Title = "Scalar Example";
    opt.Theme = ScalarTheme.Mars;
    opt.DefaultHttpClient = new(ScalarTarget.Http, ScalarClient.Http11);

    foreach (var description in provider.ApiVersionDescriptions)
    {
        opt.AddDocument(description.GroupName.ToString(), description.GroupName.ToUpper(),routePattern: "openapi/{documentName}.json");
    }
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
