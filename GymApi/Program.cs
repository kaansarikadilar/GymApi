using System.Text;
using GymApi.Data;
using GymApi.Models;
using GymApi.Repository;
using GymApi.Repository.Impl;
using GymApi.Service;
using GymApi.Service.Impl;
using GymApi.Services;
using GymApi.Services.Impl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Refit;
using GymApi.Modules.Barcode.Clients;
using GymApi.Modules.Barcode.Repository;
using GymApi.Modules.Barcode.Service;
using GymApi.Modules.Barcode.Service.BarcodeServiceImpl;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Controllers & NewtonsoftJson loop handling
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

builder.Services.AddEndpointsApiExplorer();

// Swagger configuration with JWE Security Definition
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Gym API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter your encrypted JWE token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWE",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// Database Context Setup (PostgreSQL)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddDbContext<BarcodeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BarcodeConnection")));

// Identity Setup
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Full JWE Authentication Configuration (Validates Signature & Decrypts Token Payload)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        
        // 1. Signature Verification Key
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"]!)
        ),
        
        // 2. JWE Decryption Key (Decrypts token body)
        TokenDecryptionKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JWT:DecryptionKey"]!)
        )
    };
});

// Dependency Injection Registrations
builder.Services.AddScoped<ITokenService, TokenServiceImpl>();
builder.Services.AddScoped<IAppUserService,AppUserServiceImpl>();
builder.Services.AddScoped<IMemberService,MemberServiceImpl>();
builder.Services.AddScoped<IMemberRepository,MemberRepositoryImpl>();
builder.Services.AddScoped<IBarcodeRepository, BarcodeRepositoryImpl>();
builder.Services.AddScoped<IBarcodeService,BarcodeServiceImpl>();
builder.Services
    .AddRefitClient<IBarcodeApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5082")); // Standalone Barcode server URL
builder.Services
    .AddRefitClient<IMemberApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5082")); // Standalone Barcode server URL


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/swagger/v1/swagger.json");
    });
}

app.UseHttpsRedirection();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
    //.WithOrigins(https://localhost:5082)
    .SetIsOriginAllowed(origin => true));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbInitializer.SeedAsync(services);
}

app.Run();