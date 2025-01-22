using DotNetEnv; // Load environment variables from .env file
using System.Data; // Required for database connection interface (IDbConnection)
using Microsoft.Data.SqlClient; // SQL Server connection and operations
using Microsoft.IdentityModel.Tokens; // JWT token validation
using NotesBE.Data; // Repository classes for application logic
using Microsoft.OpenApi.Models; // For Swagger API documentation
using System.Text; // Encoding utilities for JWT
using Microsoft.AspNetCore.Authentication.JwtBearer; // For JWT authentication

// Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Build the SQL Server connection string dynamically using environment variables
string connectionString = $"Server={Env.GetString("SQL_SERVER")},{Env.GetString("SQL_PORT")};" +
                          $"Database={Env.GetString("SQL_DATABASE")};" +
                          $"User Id={Env.GetString("SQL_USER")};" +
                          $"Password={Env.GetString("SQL_PASSWORD")};" +
                          "TrustServerCertificate=True;";
Console.WriteLine("Using Connection String:");
Console.WriteLine(connectionString);

// Load JWT configuration from environment variables
string jwtSecret = Env.GetString("JWT_SECRET") ?? throw new InvalidOperationException("JWT_SECRET is missing.");
string jwtIssuer = Env.GetString("JWT_ISSUER") ?? "NotesBE";
string jwtAudience = Env.GetString("JWT_AUDIENCE") ?? "NotesBEUsers";

builder.Services.AddControllers();

// Configure Swagger/OpenAPI for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NotesBE",
        Version = "v1",
        Description = "API documentation for NotesApp backend"
    });

    // Add JWT bearer authentication to Swagger UI
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

// Dependency Injection setup for repositories and database connection
builder.Services.AddScoped<NoteRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IDbConnection>(_ => new SqlConnection(connectionString));

// Configure JWT Authentication
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
            ValidateLifetime = true // Ensure tokens are not expired
        };
    });

// Configure CORS to allow all origins, methods, and headers
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Log incoming HTTP requests for debugging
app.Use(async (context, next) =>
{
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next();
});

// Enable Swagger UI in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll"); // Apply CORS policy
app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseAuthentication(); // Enable JWT authentication middleware
app.UseAuthorization(); // Enable authorization middleware
app.MapControllers(); // Map controller routes

// Test SQL Server connection during application startup
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

app.Run(); // Start the application



/*Summary:
This code sets up an ASP.NET Core Web API with:

Environment Variable Loading: For connection strings and JWT secrets.
SQL Server Connection: Dynamically built using .env values.
JWT Authentication: Validates user requests with tokens.
CORS Policy: Allows all origins and methods.
Swagger: Provides API documentation and testing.
Dependency Injection: Injects repositories and database connections for clean code separation.
Request Logging: Outputs HTTP request details during development.*/