using BudgetFlow.API.Middlewares;
using BudgetFlow.Application;
using BudgetFlow.Application.Categories.Commands.CreateCategory;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Infrastructure;
using BudgetFlow.Infrastructure.Contexts;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using FluentValidation.AspNetCore;
using BudgetFlow.Application.Portfolios.Commands.CreatePortfolio;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BudgetFlow.Infrastructure.Seed;
using BudgetFlow.Infrastructure.Repositories;
using BudgetFlow.Application.Common.Interfaces.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig.ReadFrom.Configuration(context.Configuration);
});

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// App + Infra
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// Middlewares
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// 🔐 JWT Authentication
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret is missing"))),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = JwtRegisteredClaimNames.Sub // Map `sub` as the UserID
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            }
        };
    });

// 🧩 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.CustomSchemaIds(id => id.FullName.Replace('+', '-'));

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter your JWT token in this field",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT"
    };

    o.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

    var securityRequirements = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            []
        }
    };
    o.AddSecurityRequirement(securityRequirements);

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    o.IncludeXmlComments(xmlPath);
});

// 🔌 BudgetContext DI + Configurable Npgsql
builder.Services.AddDbContext<BudgetContext>((sp, options) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    var connectionString = configuration.GetConnectionString("DbConnection");
    options.UseNpgsql(connectionString);
});



// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(CreatePortfolioValidator).Assembly);
builder.Services.AddFluentValidationAutoValidation(config =>
{
    config.DisableDataAnnotationsValidation = true;
});
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .Select(e => new { code = "Validation.Error", message = e.Value?.Errors.First().ErrorMessage })
            .ToList();

        return new BadRequestObjectResult(new
        {
            type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            title = "One or more validation errors occurred.",
            status = 400,
            error = errors.FirstOrDefault(),
            traceId = Activity.Current?.Id ?? actionContext.HttpContext.TraceIdentifier
        });
    };
});

// 🌐 CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => policy
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
});

// ============================
// 🛠️ APP BUILD & PIPELINE
// ============================

var app = builder.Build();

// 🔃 Migration + Seed işlemleri
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<BudgetContext>();

    // ➕ Migration
    context.Database.Migrate();

    // ➕ Rolleri ekle
    if (!context.Roles.Any())
    {
        context.Roles.AddRange(
            new Role { ID = Role.AdminID, Name = Role.Admin },
            new Role { ID = Role.MemberID, Name = Role.Member }
        );
        context.SaveChanges();
    }

    // 👤 Admin user seed
    var seeder = services.GetRequiredService<AdminUserSeeder>();
    await seeder.SeedAsync();
}

// Dev Tools
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BudgetFlow API V1");
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); // 🔒 Controller uçları kapalı
    });
    app.UseHttpsRedirection();
}

app.UseExceptionHandler();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
