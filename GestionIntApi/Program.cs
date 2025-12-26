
using GestionIntApi.Models;
using GestionIntApi.Repositorios.Contrato;
using GestionIntApi.Repositorios.Implementacion;
using GestionIntApi.Repositorios.Interfaces;
using GestionIntApi.Servicios.Implementacion;
using GestionIntApi.Utilidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;




/*using SistemaNutricion.Models;
using SistemaNutricion.Repository.Interfaces;
using static SistemaNutricion.Repository.Implementacion.UsuarioRepositorio;
using SistemaNutricion.Repository;
using SistemaNutricion.Utilidades;
using SistemaNutricion.Repository.Contratos;
using static SistemaNutricion.Repository.Implementacion.AlimentoRepositorio;
using static SistemaNutricion.Repository.Implementacion.EjercicioRepositorio;
using SistemaNutricion.DTO;
using SistemaNutricion.Repository.Implementacion;
*/
using System.Text.Json;
using System.Text.Json.Serialization;










var builder = WebApplication.CreateBuilder(args);
// ?? JWT ENV
builder.WebHost.UseUrls("http://0.0.0.0:7166");

/*var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new Exception("JWT_SECRET_KEY no está configurada en variables de entorno");
}

builder.Configuration["JwtSettings:SecretKey"] = jwtSecret;

var key = Encoding.UTF8.GetBytes(jwtSecret);
*/
/*var jwtSecret =
    Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? "12345678901234567890123456789012";

builder.Configuration["JwtSettings:SecretKey"] = jwtSecret;

var key = Encoding.UTF8.GetBytes(jwtSecret);
*/
var jwtSecret =
    Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? builder.Configuration["JwtSettings:SecretKey"];

if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new Exception("JWT_SECRET_KEY no configurada");
}

var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.WebHost.UseUrls("http://0.0.0.0:7166");


//var jwtSettings = builder.Configuration.GetSection("JwtSettings");
//var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };

    // ?? CLAVE PARA SIGNALR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/adminhub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();


builder.Services.AddSignalR();



builder.Configuration["SendGrid:ApiKey"] = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
//builder.Configuration["JwtSettings:SecretKey"] = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");



builder.Services.AddDbContext<SistemaGestionDBcontext>(options =>
{
options.UseNpgsql(builder.Configuration.GetConnectionString("sqlConection")
    , npgsqlOptions => npgsqlOptions.EnableRetryOnFailure());


    
    
});


//builder.Services.AddTransient(typeof(IGenrericRepository<>), typeof(GenericRepository<>));
AppContext.SetSwitch("Microsoft.Data.SqlClient.DisableCertificateValidation", true);
builder.Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
/*  AppContext.SetSwitch("Microsoft.Data.SqlClient.DisableCertificateValidation", true);*/

builder.Services.AddCors(options => {
    options.AddPolicy("NuevaPolitica", app =>
    {
        app.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowedToAllowWildcardSubdomains();
    });

}
  );


// Usar CORS

// Add services to the container.

builder.Services.AddAutoMapper(typeof(AutoMapperPerfil));
builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddSingleton<ICodigoVerificacionService, CodigoVerificacionService>();
builder.Services.AddSingleton<IRegistroTemporalService, RegistroTemporalService>();
builder.Services.AddScoped<ICreditoService, CreditoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IDetalleClienteService, DetalleClienteService>();
builder.Services.AddScoped<IClienteRepository, GenericRepositoryCliente>();
builder.Services.AddScoped<ITiendaService, TiendaService>();

builder.Services.AddScoped<INotificacionServicio,NotificacionService>();

builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddSingleton<ICodigoVerificacionService, CodigoVerificacionService>();
builder.Services.AddScoped<IUbicacionService, UbicacionService>();
builder.Services.AddSingleton<IUserIdProvider, ClienteIdProvider_cs>();
builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();

builder.Services.Configure<SendGridSettings>(
    builder.Configuration.GetSection("SendGrid"));

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.WriteIndented = true;
    //options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());*/
    //  options.JsonSerializerOptions.PropertyNamingPolicy = null; // Desactivar la pol?tica de nombres de propiedad para respetar los nombres tal como est?n en el DTO
    // Agregar un convertidor personalizado si es necesario

    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.WriteIndented = true;
    //options.JsonSerializerOptions.Converters.Add(new JsonDateTimeConverter("dd/MM/yyyy"));

});



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("NuevaPolitica");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<AdminHub>("/adminhub");


app.Run();