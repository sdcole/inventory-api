using InventoryAPI.Models;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog configuration
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.PostgreSQL(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        tableName: "logs",
        needAutoCreateTable: true, // This will auto-create the Logs table if it doesn't exist
        useCopy: false,
        columnOptions: new Dictionary<string, ColumnWriterBase>
        {
            { "Message", new RenderedMessageColumnWriter() },
            { "MessageTemplate", new MessageTemplateColumnWriter() },
            { "Level", new LevelColumnWriter() },
            { "TimeStamp", new TimestampColumnWriter() },
            { "Exception", new ExceptionColumnWriter() },
            { "Properties", new PropertiesColumnWriter() }
        }
    )
    .CreateLogger();
try
{
    // Log a message indicating that the web application has started
    logger.Error("This is annoying");
}
catch (Exception ex)
{
    Console.WriteLine($"Error logging startup message: {ex.Message}");
}
builder.Host.UseSerilog(logger);

// Add policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "DefaultPolicy",
        policy =>
        {
            policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ConfigSettings>(builder.Configuration.GetSection("ConfigSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors("DefaultPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();