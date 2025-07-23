using Application.Application.Extensions;
using Application.Domain.Entities;
using Application.EntityFrameworkCore;
using Application.EntityFrameworkCore.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json", optional: false);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("Default")));

        services.AddApplicationServices();

    })
    .Build();

using var scope = host.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

Console.WriteLine("Applying Migrations...");
await dbContext.Database.MigrateAsync();

Console.WriteLine("Seeding Permissions...");
await PermissionSeeder.SeedAsync(dbContext);

Console.WriteLine("Done!");
