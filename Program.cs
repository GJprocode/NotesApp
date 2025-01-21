using DotNetEnv; // Load environment variables from .env file
using System.Data; // Required for IDbConnection
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using NotesAppBackend.Data; // Import the namespace for repositories
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

Env.Load(); // Load environment variables

var builder = WebApplication.CreateBuilder(args);

// Build the connection string dynamically from .env variables
string connectionString = $"Server={Env.GetString("SQL_SERVER")},{Env.GetString("SQL_PORT")};" +
                          $"Database={Env.GetString("SQL_DATABASE")};" +
                          $"User Id={Env.GetString("SQL_USER")};" +
                          $"Password={Env.GetString("SQL_PASSWORD")};" +
                          "TrustServerCertificate=True;";
Console.WriteLine("Using Connection String:");
Console.WriteLine(connectionString);

// Load JWT configuration
string jwtSecret = Env.GetString("JWT_SECRET") ?? throw new InvalidOperationException("JWT_SECRET is missing.");
string jwtIssuer = Env.GetString("JWT_ISSUER") ?? "NotesAppBackend";
string jwtAudience = Env.GetString("JWT_AUDIENCE") ?? "NotesAppBackendUsers";

// Register services and middleware
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NotesAppBackend",
        Version = "v1",
        Description = "API documentation for NotesApp backend"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token like 'Bearer {your token}'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
builder.Services.AddScoped<NoteRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true
        };
    });

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    using (var connection = new SqlConnection(connectionString))
    {
        connection.Open();
        Console.WriteLine("Connected to SQL Server successfully!");
    }
}
catch (SqlException ex)
{
    Console.WriteLine($"Error connecting to SQL Server: {ex.Message}");
}

app.Run();

/*
 * NOTES:
 * - Swagger auto-detects models from controller actions.
 * - Ensure controllers use `User` and `Note` models in request/response types.
 * - Use Swagger instead of Postman for testing endpoints!
 */

/*
 * NOTES FOR FUTURE EXPANSION
 * --------------------------
 * - Authentication: Add support for JWT authentication and user login/register endpoints.
 * - Authorization: Implement role-based access control (RBAC) for endpoints (e.g., admin vs. regular user).
 * - Validation: Add model validation using Data Annotations or FluentValidation.
 * - Exception Handling: Use a global exception handler to return consistent error responses.
 * - Configuration: Move static configurations to appsettings.json or environment variables for better management.
 * - Caching: Implement caching for frequently used data like notes and user details.
 * - Deployment: Ensure production configurations are set, such as disabling Swagger in production.
 * 
 * ADDITIONAL FEATURES:
 * --------------------
 * - Login and Registration: Add endpoints for user registration, login, and logout.
 * - User Authentication: Secure APIs using authentication tokens (e.g., JWT).
 * - Improved Logging: Integrate a logging library (e.g., Serilog) for advanced logging needs.
 * - Testing: Add comprehensive unit and integration tests for APIs and repositories.
 */
