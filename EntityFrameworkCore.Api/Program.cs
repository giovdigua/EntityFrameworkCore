using EntityFrameworkCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Fix cicly retain
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("SqlDatabaseConnectionString");

builder.Services.AddDbContext<FootballLeagueDbContext>(options =>
{
    options.UseSqlServer(connectionString)
                                         //.UseLazyLoadingProxies() // For use Lazy Loading after install ef.Proxie package ,decommnet only if you wnat lazy loading(not recommended)
                                         //optionsBuilder.UseSqlite($"Data Source={DBPath}")
                                         .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) // for global no tracking
                .LogTo(Console.WriteLine, LogLevel.Information);
    if (!builder.Environment.IsProduction())
    {
        //Not in productions
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
