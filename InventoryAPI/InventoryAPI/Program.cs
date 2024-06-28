using InventoryAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using System.Collections.Generic;
using System.Text;

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
    logger.Information("Web API Starting");
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

// Configure JWT authentication
var jwtConfig = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtConfig["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Register JwtHelper
builder.Services.AddSingleton<JwtHelper>();

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

app.UseAuthentication(); // Add this line to enable authentication middleware
app.UseAuthorization();

app.MapControllers();

app.Run();