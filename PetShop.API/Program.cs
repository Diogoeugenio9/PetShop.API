using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetShop.API.Data;
using PetShop.API.Repository;
using PetShop.API.Repository.Interface;
using PetShop.API.Services;
using PetShop.API.Services.Agenda;
using PetShop.API.Services.Agendamento;
using PetShop.API.Services.ClienteAutenticacao;
using PetShop.API.Services.Configuracao;
using PetShop.API.Services.Cliente;
using PetShop.API.Services.Dashboard;
using PetShop.API.Services.Lancamento;
using PetShop.API.Services.Pet;
using PetShop.API.Services.Portal;
using PetShop.API.Services.Produto;
using PetShop.API.Services.Servico;
using PetShop.API.Services.Vacina;
using PetShop.API.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var erros = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .ToDictionary(
                    e => e.Key,
                    e => e.Value.Errors
                        .Select(x => string.IsNullOrWhiteSpace(x.ErrorMessage) ? "Valor inválido." : x.ErrorMessage)
                        .ToArray());

            var primeira = erros.Values.SelectMany(v => v).FirstOrDefault() ?? "Dados inválidos.";
            return new BadRequestObjectResult(new { message = primeira, mensagem = primeira, errors = erros });
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Informe o token JWT"
    });

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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Politicas.Administrador, policy => policy
        .RequireAuthenticatedUser()
        .RequireAssertion(context => !context.User.EhCliente() && context.User.ObterAdministradorId() > 0));

    options.AddPolicy(Politicas.Cliente, policy => policy
        .RequireAuthenticatedUser()
        .RequireAssertion(context => context.User.EhCliente()
                                     && context.User.ObterClienteId() > 0
                                     && context.User.ObterPetshopId() > 0));
});

var origensPermitidas = builder.Configuration.GetSection("Cors:Origens").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFront", policy =>
    {
        if (origensPermitidas.Length > 0)
            policy.WithOrigins(origensPermitidas);
        else
            policy.AllowAnyOrigin();

        policy.AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("autenticacao", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "desconhecido",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IPetModeloService, PetModeloService>();
builder.Services.AddScoped<IServicoService, ServicoService>();
builder.Services.AddScoped<IAgendamentoService, AgendamentoService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<ILancamentoService, LancamentoService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IClienteAuthService, ClienteAuthService>();
builder.Services.AddScoped<IPortalClienteService, PortalClienteService>();
builder.Services.AddScoped<IVacinaService, VacinaService>();
builder.Services.AddScoped<IConfiguracaoService, ConfiguracaoService>();
builder.Services.AddScoped<IAgendaService, AgendaService>();
builder.Services.AddScoped<IConclusaoAgendamentoService, ConclusaoAgendamentoService>();

if (builder.Configuration.GetValue("ConclusaoAutomatica:Habilitada", true))
    builder.Services.AddHostedService<ConclusaoAutomaticaWorker>();

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IPetModeloRepository, PetModeloRepository>();
builder.Services.AddScoped<IServicoRepository, ServicoRepository>();
builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
builder.Services.AddScoped<IAdministradorRepository, AdministradorRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<ILancamentoRepository, LancamentoRepository>();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

app.UseExceptionHandler(appErro => appErro.Run(async context =>
{
    var erro = context.Features.Get<IExceptionHandlerFeature>()?.Error;

    var (status, mensagem) = erro switch
    {
        RegraDeNegocioException regra => (StatusCodes.Status400BadRequest, regra.Message),
        ConflitoException conflito => (StatusCodes.Status409Conflict, conflito.Message),
        AcessoNegadoException negado => (StatusCodes.Status403Forbidden, negado.Message),
        DbUpdateException dbErro when ConclusaoAgendamentoService.EhViolacaoDeUnicidade(dbErro)
            => (StatusCodes.Status409Conflict, "Registro duplicado: já existe um registro com estes dados."),
        DbUpdateException => (StatusCodes.Status409Conflict, "Não foi possível salvar: o registro está ligado a outros dados."),
        _ => (StatusCodes.Status500InternalServerError, "Ocorreu um erro inesperado. Tente novamente.")
    };

    if (status == StatusCodes.Status500InternalServerError || erro is DbUpdateException)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(erro, "Erro ao processar {Metodo} {Caminho}", context.Request.Method, context.Request.Path);
    }

    context.Response.StatusCode = status;
    await context.Response.WriteAsJsonAsync(new RespostaErro(mensagem));
}));

if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Swagger:Habilitado"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFront");

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
