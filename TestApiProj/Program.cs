using TestApiProj.DataAccess;
using TestApiProj.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;  
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using TestApiProj.Models;
using TestApiProj.Models.FakerapiModel;
using Microsoft.EntityFrameworkCore;
using TestApiProj.Middlewares;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],  // Ensure these keys exist in your appsettings.json
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]))
    };
});

//Configure Roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User"));
    options.AddPolicy("SuperAdmin", policy => policy.RequireRole("SuperAdmin"));
});

////Enable cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", policy =>
    {
        policy.AllowAnyOrigin() // This allows any origin
              .AllowAnyMethod()  // This allows any HTTP method (GET, POST, etc.)
              .AllowAnyHeader();  // This allows any header
    });
});

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowVercel",
//        policy =>
//        {
//            policy
//                .WithOrigins(
//                    "https://react-opensource-project-forntend-v.vercel.app"
//                )
//                .AllowAnyHeader()
//                .AllowAnyMethod();
//        });
//});

// Add Swagger services
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
            new string[] { }
        }
    });
});

// Register memory caching
builder.Services.AddMemoryCache();
// DI
builder.Services.AddScoped<IOperations, Operations>();
builder.Services.AddScoped<IInvoice, PdfInvoiceGenerator>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Configure Serilog
Log.Logger = new LoggerConfiguration()
 .ReadFrom.Configuration(builder.Configuration)
 .CreateLogger();

builder.Host.UseSerilog();



var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
app.UseAuthentication();  // Add authentication middleware
app.UseAuthorization();   // Add authorization middleware

// Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI();
// Enable CORS globally
app.UseCors("AllowAnyOrigin");
//app.UseCors("AllowVercel");

app.UseHttpsRedirection();

// Map controllers for the API
app.MapControllers();


//Minimal Api
app.MapGet("/call-minimalApi", async (IHttpClientFactory httpClientFactory) =>
{
    // Create a client using IHttpClientFactory
    var client = httpClientFactory.CreateClient();

    // Define the external API URL
    var url = "https://fakerapi.it/api/v1/users?locale=en_US&seed=12345";  // External API

    // Send a GET request to fetch the users
    HttpResponseMessage response = await client.GetAsync(url);

    if (response.IsSuccessStatusCode)
    {
        string responseBody = await response.Content.ReadAsStringAsync();

        // Deserialize the response body into a list of EmployeeResponse objects
        var employeeData = JsonConvert.DeserializeObject<EmployeeResponse>(responseBody);

        // Return the deserialized data as JSON to the client
        return Results.Ok(employeeData);
    }

    // Return an empty list or appropriate error response if API fails
    return Results.NotFound(new List<EmployeeResponse>());
});


app.Run();
