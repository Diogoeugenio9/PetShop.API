using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetShop.API.Data;
using PetShop.API.Mappings;
using PetShop.API.Repositories.Cliente;
using PetShop.API.Repository;
using PetShop.API.Repository.Interface;
using PetShop.API.Services;
using PetShop.API.Services.Agendamento;
using PetShop.API.Services.Cliente;
using PetShop.API.Services.Pet;
using PetShop.API.Services.Servico;
using PetShop.API.Services.Servico.PetShop.API.Services.Servico;

// ❌ LINHA ERRADA REMOVIDA
// using PetShop.API.Services.Servico.PetShop.API.Services.Servico;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            )
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFront",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

// Services
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IPetModeloService, PetModeloService>();
builder.Services.AddScoped<IServicoService, ServicoService>();
builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Repositories
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IPetModeloRepository, PetModeloRepository>();
builder.Services.AddScoped<IServicoRepository, ServicoRepository>();
builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
builder.Services.AddScoped<IAdministradorRepository, AdministradorRepository>();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ❌ HTTPS pode dar problema no Railway (opcional remover)
app.UseHttpsRedirection();

// CORS
app.UseCors("AllowFront");

// Auth
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


// 🔥 CORREÇÃO PRINCIPAL PRO RAILWAY
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Run($"http://0.0.0.0:{port}");