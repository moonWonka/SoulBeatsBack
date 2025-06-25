using Microsoft.AspNetCore.Builder;
using BackendSoulBeats.IntegrationTests.Application;

var builder = WebApplication.CreateBuilder(args);

var startup = new TestStartup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

app.Run();
