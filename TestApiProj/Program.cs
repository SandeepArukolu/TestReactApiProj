
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using System.Text;
using TestApiProj.DataAccess;
using TestApiProj.Mapping;
using TestApiProj.Middlewares;
using TestApiProj.Models;
using TestApiProj.Models.FakerapiModel;
using TestApiProj.Services;
using AutoMapper;

// Disable file watchers for Render
Environment.SetEnvironmentVariable(
    "DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE",
    "false");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? string.Empty))
    };
});

// Configure Roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User"));
    options.AddPolicy("SuperAdmin", policy => policy.RequireRole("SuperAdmin"));
});

////// Enable CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAnyOrigin", policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyMethod()
//              .AllowAnyHeader();
//    });
//});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowFrontend", policy =>
//    {
//        policy.WithOrigins(
//            "https://react-opensource-project-forntend-m.vercel.app"
//        )
//        .AllowAnyHeader()
//        .AllowAnyMethod();
//    });
//});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter your token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Memory cache
builder.Services.AddMemoryCache();

// DI
builder.Services.AddScoped<IOperations, Operations>();
builder.Services.AddScoped<IInvoice, PdfInvoiceGenerator>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowLocalOrigin");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();

app.UseSwaggerUI();



// Minimal API
app.MapGet("/call-minimalApi", async (IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();

    var url = "https://fakerapi.it/api/v1/users?locale=en_US&seed=12345";

    HttpResponseMessage response = await client.GetAsync(url);

    if (response.IsSuccessStatusCode)
    {
        string responseBody = await response.Content.ReadAsStringAsync();

        var employeeData =
            JsonConvert.DeserializeObject<EmployeeResponse>(responseBody);

        return Results.Ok(employeeData);
    }

    return Results.NotFound();
});

// Render Port Configuration
var port = Environment.GetEnvironmentVariable("PORT");

if (!string.IsNullOrEmpty(port))
{
    app.Urls.Add($"http://0.0.0.0:{port}");
}

app.Run();