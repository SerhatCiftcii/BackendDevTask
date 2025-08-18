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

// SeriLog'u yapýlandýrýyoruz.
// Normalde bu kadar uzun olmaz, ama her þeyi adým adým ekliyoruz.
// SeriLog'u yapýlandýrýyoruz.
builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostingContext.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .MinimumLevel.Information() // Varsayýlan log seviyesini Warning'e çektik
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)  // Microsoft loglarýný warning seviyesinden düþükse gösterme ***  bunu yapa sebebim log dosyasý çok þiþiyor.
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)); // EF Core loglarýný warning seviyesinden düþükse gösterme

// PostgreSQL veritabaný baðlantýmýzý ve EF Core servisimizi ekliyoruz.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis için servisimizi ekliyoruz.
// Hata oluþmamasý için `appsettings.json` dosyasýnda "RedisConnection" olduðundan emin ol.
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "myredisinstance"; // Buraya bir isim verebilirsin, fark etmez.
});

// MediatR'ý ve tüm handler'larýmýzý (komut ve sorgu iþleyicilerimiz) ekliyoruz.
// `typeof(RegisterUserCommand).Assembly` ile Application katmanýndaki tüm handler'larý buluyor.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));

// Kendi yazdýðýmýz repository'leri ve servisleri buraya ekliyoruz.
// `Scoped` ile her HTTP isteði için yeni bir örnek oluþturulacak.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// JWT kimlik doðrulamasýný (Authentication) ayarlýyoruz.
// Hangi þemayý kullanacaðýmýzý belirtiyoruz.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
// JWT Bearer token'ý için spesifik ayarlar yapýyoruz.
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Token'ýn verenini (Issuer) doðrula.
        ValidateIssuer = true,
        // Token'ýn alýcýsýný (Audience) doðrula.
        ValidateAudience = true,
        // Token'ýn geçerlilik süresini doðrula.
        ValidateLifetime = true,
        // Ýmza anahtarýný doðrula.
        ValidateIssuerSigningKey = true,

        // Doðru Issuer ve Audience deðerlerini `appsettings.json`'dan alýyoruz.
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        // Ýmza anahtarýný da yine oradan alýyoruz.
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Artýk ASP.NET Core MVC Controller'larýmýzý kullanabiliriz.
builder.Services.AddControllers();

// Swagger (API dokümantasyonu) ve JWT için Swagger ayarlarýný yapýyoruz.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Swagger belgesinin adýný ve baþlýðýný ayarlýyoruz.
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "MyProject API", Version = "v1" });

    // Swagger UI'da JWT token'ý girebilmek için güvenlik tanýmýný ekliyoruz.
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    // Hangi API endpoint'lerinin bu güvenlik tanýmýný kullanacaðýný belirtiyoruz.
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