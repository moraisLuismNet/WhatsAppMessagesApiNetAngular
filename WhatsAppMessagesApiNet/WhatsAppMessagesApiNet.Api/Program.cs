using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using WhatsAppMessagesApiNet.Api.Middleware;
using WhatsAppMessagesApiNet.Application.Interfaces;
using WhatsAppMessagesApiNet.Application.Mappings;
using WhatsAppMessagesApiNet.Application.Services;
using WhatsAppMessagesApiNet.Domain.Interfaces;
using WhatsAppMessagesApiNet.Infrastructure.Audit;
using WhatsAppMessagesApiNet.Infrastructure.Persistence.EF;
using WhatsAppMessagesApiNet.Infrastructure.Persistence.Mongo;
using WhatsAppMessagesApiNet.Infrastructure.Persistence.Mongo.Mappings;
using WhatsAppMessagesApiNet.Infrastructure.Persistence.Repositories;
using WhatsAppMessagesApiNet.Infrastructure.Providers;
using WhatsAppMessagesApiNet.Infrastructure.Services;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

    var dbProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";

    // Database Configuration
    if (dbProvider.Equals("MongoDB", StringComparison.OrdinalIgnoreCase))
    {
        MongoClassMaps.Register();
        builder.Services.AddSingleton<MongoDbContext>();
        builder.Services.AddScoped<IUnitOfWork, MongoUnitOfWork>();
    }
    else
    {
        builder.Services.AddSingleton<AuditInterceptor>();
        builder.Services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditInterceptor>();
            options.AddInterceptors(interceptor);

            var connectionString = builder.Configuration.GetConnectionString(dbProvider);
            switch (dbProvider.ToLower())
            {
                case "mysql":
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    break;
                case "postgresql":
                    options.UseNpgsql(connectionString);
                    break;
                case "sqlite":
                    options.UseSqlite(connectionString);
                    break;
                default:
                    options.UseSqlServer(connectionString);
                    break;
            }
        });

        builder.Services.AddScoped<IUnitOfWork, EFUnitOfWork>();
    }

    // Application Services
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IMessageService, MessageService>();
    builder.Services.AddScoped<IAuthService, AuthService>();

    // WhatsApp Providers (BSPs)
    builder.Services.AddTransient<IWhatsAppProvider>(sp =>
    {
        var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
        var config = sp.GetRequiredService<IConfiguration>();
        return new TwilioWhatsAppProvider(httpClientFactory.CreateClient(), config,
            sp.GetRequiredService<ILogger<TwilioWhatsAppProvider>>());
    });
    builder.Services.AddHttpClient();

    // AutoMapper
    builder.Services.AddAutoMapper(typeof(MappingProfile));

    // FluentValidation
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssembly(typeof(WhatsAppMessagesApiNet.Application.Validators.CreateUserValidator).Assembly);

    // JWT Authentication
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

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
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ClockSkew = TimeSpan.Zero
        };
    });

    builder.Services.AddAuthorization();

    // CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp", policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
    });

    // Controllers + Swagger
    builder.Services.AddControllers()
        .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.EnableAnnotations();
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "WhatsAppMessagesApiNet",
            Version = "v1",
            Description = "API for sending WhatsApp messages"
        });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token"
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
                Array.Empty<string>()
            }
        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
            options.IncludeXmlComments(xmlPath);
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseMiddleware<RequestLoggingMiddleware>();

    // Auto-migrate and seed for relational databases
    using (var scope = app.Services.CreateScope())
    {
        if (!dbProvider.Equals("MongoDB", StringComparison.OrdinalIgnoreCase))
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (app.Environment.IsDevelopment())
            {
                Log.Information("Ensuring database is created...");
                context.Database.EnsureCreated();
                Log.Information("Database ready");
            }
            else
            {
                context.Database.Migrate();
            }
        }

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        await WhatsAppMessagesApiNet.Infrastructure.Persistence.EF.Seed.DbInitializer.SeedAsync(unitOfWork);
    }

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors("AllowAngularApp");
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("WhatsAppMessagesApiNet started with provider: {Provider}", dbProvider);

    var swaggerUrl = builder.Configuration["Urls"] ?? "http://localhost:5034";
    Log.Information("Opening Swagger UI at {SwaggerUrl}/swagger", swaggerUrl);

    _ = Task.Run(async () =>
    {
        await Task.Delay(2000);
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = $"{swaggerUrl}/swagger",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Could not open browser automatically");
        }
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
