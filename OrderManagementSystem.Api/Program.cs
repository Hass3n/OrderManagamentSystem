using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderManagementSystem.Api.Middleware;
using OrderManagementSystem.Application;
using OrderManagementSystem.Application.Mapping;
using OrderManagementSystem.Application.Services;
using OrderManagementSystem.Domain.Interfaces;
using OrderManagementSystem.Domain.Services;
using OrderManagementSystem.Infrastructure.Data;
using OrderManagementSystem.Infrastructure.Repositories;
using OrderManagementSystem.Infrastructure.Services;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;


        var builder = WebApplication.CreateBuilder();

        // Add services to the container.
        builder.Services.AddControllers();

        // Configure Entity Framework with In-Memory Database
        builder.Services.AddDbContext<OrderManagementDbContext>(options =>
            options.UseInMemoryDatabase("OrderManagementDb"));

// Configure AutoMapper
//builder.Services.AddAutoMapper(AppDomain.MonitoringIs);
        builder.Services.AddAutoMapper(typeof(MappingProfile));
//builder.Services.AddApplicationServices(builder.Configuration);



// Register repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register domain services
        builder.Services.AddScoped<IDiscountService, DiscountService>();

        // Register application services
        builder.Services.AddScoped<ICustomerService, CustomerService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IInvoiceService, InvoiceService>();

        // Register infrastructure services
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IJwtService>(provider =>
            new JwtService(
                secretKey: "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
                issuer: "OrderManagementSystem",
                audience: "OrderManagementSystemUsers"
            ));


// Configure JWT Authentication
var jwtSettings = new
        {
            SecretKey = "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
            Issuer = "OrderManagementSystem",
            Audience = "OrderManagementSystemUsers"
        };

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.SecretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization();

        // Configure Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Order Management System API",
                Version = "v1",
                Description = "A comprehensive Order Management System API with role-based access control, tiered discounts, and invoice generation.",
                Contact = new OpenApiContact
                {
                    Name = "Order Management System",
                    Email = "support@ordermanagementsystem.com"
                }
            });

            // Add JWT Authentication to Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
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

            // Include XML comments
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        // Configure CORS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        // Seed the database
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();
            context.Database.EnsureCreated();
        }

        // Configure the HTTP request pipeline.
        app.UseMiddleware<ErrorHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Management System API v1");
               
            });
        }

        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
   