using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var staticFilesPath = Directory.GetCurrentDirectory();
var provider = new PhysicalFileProvider(staticFilesPath);

app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = provider,
    DefaultFileNames = { "index.html" }
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = provider,
    RequestPath = ""
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoice API V1");
    c.RoutePrefix = "swagger";
});

app.MapControllers();

app.Run();
