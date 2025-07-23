using Application.Application.Extensions;
using Application.Application.Services.Auth;
using Application.Application.Services.Common;
using Application.Contracts.Interfaces.Auth;
using Application.Domain.Entities;
using Application.Domain.Shared.Interfaces;
using Application.EntityFrameworkCore;
using Application.EntityFrameworkCore.Seed;
using Application.HttpApi.Extensions;
using Application.HttpApi.Host;
using Application.HttpApi.Host.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
        });

        // Add services to the container.
        builder.Services.AddRazorPages();

        builder.Services.AddApplicationServices();

        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
                };
            });

        builder.Services.AddSwaggerGen(options =>
        {
            options.DocumentFilter<DynamicSwaggerGenerator>();
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");


        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();
            await PermissionSeeder.SeedAsync(context);
        }

        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en"),
            SupportedCultures = new[] { new CultureInfo("en"), new CultureInfo("ar") },
            SupportedUICultures = new[] { new CultureInfo("en"), new CultureInfo("ar") }
        });

        app.UseHttpsRedirection();
        app.UseGlobalExceptionHandling();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            foreach (var (route, method, iface) in AppServiceMetadata.Discover())
            {
                endpoints.MapPost(route, async context =>
                {
                    var service = context.RequestServices.GetRequiredService(iface);
                    var args = new List<object>();

                    foreach (var param in method.GetParameters())
                    {
                        var arg = await context.Request.ReadFromJsonAsync(param.ParameterType);
                        args.Add(arg);
                    }

                    var task = method.Invoke(service, args.ToArray()) as Task;
                    await task;

                    var result = task?.GetType().GetProperty("Result")?.GetValue(task);
                    await context.Response.WriteAsJsonAsync(result);
                });
            }
        });

        app.MapRazorPages().WithStaticAssets();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.Run();
    }
}