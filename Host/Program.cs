using Infrastructure.Di;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using SixLabors.ImageSharp;
using Steeltoe.Discovery.Client;
using Steeltoe.Extensions.Configuration;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureService(builder.Configuration);
builder.Services.AddHealthChecks();
builder.Services.AddDiscoveryClient(builder.Configuration);
//builder.Services.AddAuthentication();
builder.Environment.ApplicationName = builder.Configuration["Application:Name"];

var app = builder.Build();
app.Services.InitializeDatabasesAsync().Wait();
app.UseInFrastructure();
Console.WriteLine($"Application Name: {app.Environment.ApplicationName}");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/health");

//app.UseDiscoveryClient();

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
