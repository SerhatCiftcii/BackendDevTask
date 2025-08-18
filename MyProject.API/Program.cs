using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyProject.Application.Features.Auth.Commands;
using MyProject.Core.Interfaces;
using MyProject.Infrastructure.Data;
using MyProject.Infrastructure.Repositories;
using MyProject.Infrastructure.Services;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// SeriLog'u yap�land�r�yoruz.
// Normalde bu kadar uzun olmaz, ama her �eyi ad�m ad�m ekliyoruz.
// SeriLog'u yap�land�r�yoruz.
builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostingContext.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information() // Varsay�lan log seviyesini Warning'e �ektik
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)  // Microsoft loglar�n� warning seviyesinden d���kse g�sterme ***  bunu yapa sebebim log dosyas� �ok �i�iyor.
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)); // EF Core loglar�n� warning seviyesinden d���kse g�sterme

// PostgreSQL veritaban� ba�lant�m�z� ve EF Core servisimizi ekliyoruz.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis i�in servisimizi ekliyoruz.
// Hata olu�mamas� i�in `appsettings.json` dosyas�nda "RedisConnection" oldu�undan emin ol.
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "myredisinstance"; // Buraya bir isim verebilirsin, fark etmez.
});

// MediatR'� ve t�m handler'lar�m�z� (komut ve sorgu i�leyicilerimiz) ekliyoruz.
// `typeof(RegisterUserCommand).Assembly` ile Application katman�ndaki t�m handler'lar� buluyor.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

// Kendi yazd���m�z repository'leri ve servisleri buraya ekliyoruz.
// `Scoped` ile her HTTP iste�i i�in yeni bir �rnek olu�turulacak.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// JWT kimlik do�rulamas�n� (Authentication) ayarl�yoruz.
// Hangi �emay� kullanaca��m�z� belirtiyoruz.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
// JWT Bearer token'� i�in spesifik ayarlar yap�yoruz.
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Token'�n verenini (Issuer) do�rula.
        ValidateIssuer = true,
        // Token'�n al�c�s�n� (Audience) do�rula.
        ValidateAudience = true,
        // Token'�n ge�erlilik s�resini do�rula.
        ValidateLifetime = true,
        // �mza anahtar�n� do�rula.
        ValidateIssuerSigningKey = true,

        // Do�ru Issuer ve Audience de�erlerini `appsettings.json`'dan al�yoruz.
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        // �mza anahtar�n� da yine oradan al�yoruz.
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Art�k ASP.NET Core MVC Controller'lar�m�z� kullanabiliriz.
builder.Services.AddControllers();

// Swagger (API dok�mantasyonu) ve JWT i�in Swagger ayarlar�n� yap�yoruz.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Swagger belgesinin ad�n� ve ba�l���n� ayarl�yoruz.
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MyProject API", Version = "v1" });

    // Swagger UI'da JWT token'� girebilmek i�in g�venlik tan�m�n� ekliyoruz.
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    // Hangi API endpoint'lerinin bu g�venlik tan�m�n� kullanaca��n� belirtiyoruz.
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();


app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.Run();