using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Mappings;
using PetShop.API.Repositories.Cliente;
using PetShop.API.Repository;
using PetShop.API.Repository.Interface;
using PetShop.API.Services.Cliente;
using PetShop.API.Services.Pet;
using PetShop.API.Services.Servico;
using PetShop.API.Services.Servico.PetShop.API.Services.Servico;


//using PetShop.API.Services.Pet;
using static PetShop.API.Repository.Interface.IClienteRepository;
using static PetShop.API.Repository.PetModeloRepository;
using IClienteRepository = PetShop.API.Repository.Interface.IClienteRepository.IClienteRepository;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Services
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IPetModeloService, PetModeloService>();
builder.Services.AddScoped<IServicoService, ServicoService>();

//Repositories
builder.Services.AddScoped<IPetModeloRepository, PetModeloRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IServicoRepository, ServicoRepository>();

//AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Teste
builder.Services.AddAutoMapper(typeof(ServicoProfile));


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
