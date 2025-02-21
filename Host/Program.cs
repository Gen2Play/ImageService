using Infrastructure.Di;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureService(builder.Configuration);
//builder.Services.AddAuthentication();
builder.Environment.ApplicationName = builder.Configuration["Application:Name"];

var app = builder.Build();
app.Services.InitializeDatabasesAsync().Wait();
app.UseInFrastructure();
Console.WriteLine($"Application Name: {app.Environment.ApplicationName}");
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
