using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using CoWorkSpace.Api.Data;
using CoWorkSpace.Api.Services; 
using CoWorkSpace.Api.Models;
using CoWorkSpace.Api.Repositories;
using CoWorkSpace.Api.Services.Interfaces;
using CoWorkSpace.Api.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Configuración CORS (importante al usar cookies, AllowCredentials es necesario)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); 
    });
});

// Añadir DbContext
builder.Services.AddDbContext<CoWorkSpaceContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Registrar servicios
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IRegisterService, RegisterService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IProviderService, ProviderService>();
builder.Services.AddScoped<IProviderSpaceService, ProviderSpaceService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ISpaceBookingService, SpaceBookingService>();
builder.Services.AddScoped<IStatsService, StatsService>();

// Registrar IHttpContextAccessor para IP y otros datos HTTP
builder.Services.AddHttpContextAccessor();

// Registrar servicios de tokens / repositorios
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// Opcional: cache si necesitamos almacenamiento temporal
// builder.Services.AddDistributedMemoryCache();

// Leer configuración JWT desde appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

// Configurar autenticación JWT
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
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };

    // Si quieres que Swagger/postman puedan enviar tokens desde la UI no hace falta más aquí.
});

// Controllers + JSON
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CoWorkSpace API",
        Version = "v1"
    });
});

var app = builder.Build();

// Middleware para entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoWorkSpace API v1");
    });
}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

// El orden debe ser primero autenticación y luego autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
