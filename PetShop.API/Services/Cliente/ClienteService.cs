using Microsoft.EntityFrameworkCore;
using PetShop.API.Data;
using PetShop.API.Dto.Cliente;
using PetShop.API.Models;

namespace PetShop.API.Services.Cliente
{
    public class ClienteService : IClienteInterface
    {

        private readonly AppDbContext _context;
        public ClienteService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<ResponseModel<ClienteModel>> BuscarClientePorId(int idCliente)
        {
            ResponseModel<ClienteModel> resposta = new ResponseModel<ClienteModel>();
            try
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(clienteBanco => clienteBanco.Id == idCliente);

                if(cliente == null)
                {
                    resposta.Mensagem = "Nenhum registro localizado!";
                    return resposta;
                }

                resposta.Dados = cliente;
                resposta.Mensagem = "Autor Localizado!";

                return resposta;

            }
            catch (Exception ex)
            {
                resposta.Mensagem = ex.Message;
                resposta.Status = false;
                return resposta;
            }
        }

        public async Task<ResponseModel<ClienteModel>> BuscarClientePorIdPet(int idPet)
        {
            ResponseModel<ClienteModel> resposta = new ResponseModel<ClienteModel>();
            try
            {
                var pet = await _context.Pets
                    .Include(a => a.Cliente)
                    .FirstOrDefaultAsync(petBanco => petBanco.Id == idPet);

                if(pet == null)
                {
                    resposta.Mensagem = "Nenhum registro localizado!";
                    return resposta;

                }

                resposta.Dados = pet.Cliente;
                resposta.Mensagem = "Cliente Localizado!";
                return resposta;

            }
            catch (Exception ex)
            {
                resposta.Mensagem = ex.Message;
                resposta.Status = false;
                return resposta;
            }
            ;
        }

        public async Task<ResponseModel<List<ClienteModel>>> CriarCliente(ClienteCriacaoDto clienteCriacaoDto)
        {
            ResponseModel<List<ClienteModel>> resposta = new ResponseModel<List<ClienteModel>>();

            try
            {
                var cliente = new ClienteModel()
                {
                    Nome = clienteCriacaoDto.Nome,
                    Sobrenome = clienteCriacaoDto.Sobrenome
                };

                _context.Add(cliente);
                await _context.SaveChangesAsync();

                resposta.Dados = await _context.Clientes.ToListAsync();
                resposta.Mensagem = "Cliente criado com sucesso!";
                return resposta;

            }
            catch (Exception ex)
            {
                resposta.Mensagem = ex.Message;
                resposta.Status = false;
                return resposta;


            }

        }

        public async Task<ResponseModel<List<ClienteModel>>> EditarCliente(ClienteEdicaoDto clienteEdicaoDto)
        {
            ResponseModel<List<ClienteModel>> resposta = new ResponseModel<List<ClienteModel>>();

            try
            {
                var cliente = await _context.Clientes
                    .FirstOrDefaultAsync(clienteBanco => clienteBanco.Id == clienteEdicaoDto.Id);


                if (cliente == null)
                {
                    resposta.Mensagem = "Nenhum cliente localizado!";
                    return resposta;
                }

                cliente.Nome = clienteEdicaoDto.Nome;
                cliente.Sobrenome = clienteEdicaoDto.Sobrenome;

                _context.Update(cliente);
                await _context.SaveChangesAsync();

                resposta.Dados = await _context.Clientes.ToListAsync();
                resposta.Mensagem = "Cliente ediatdo com sucesso!";

                return resposta;


            }
            catch (Exception ex)
            {
                resposta.Mensagem = ex.Message;
                resposta.Status = false;
                return resposta;
            }

        }

        public async Task<ResponseModel<List<ClienteModel>>> ExcluirCliente(int idCliente)
        {
            ResponseModel<List<ClienteModel>> resposta = new ResponseModel<List<ClienteModel>>();

            try
            {
                var cliente = await _context.Clientes
                    .FirstOrDefaultAsync(clienteBanco => clienteBanco.Id == idCliente);

                if(cliente == null)
                {
                    resposta.Mensagem = "Nenhum cliente localizado!";
                    return resposta;
                }

                _context.Remove(cliente);
                await _context.SaveChangesAsync();

                resposta.Dados = await _context.Clientes.ToListAsync();
                resposta.Mensagem = "Cliente removido com sucesso!";

                return resposta;

            }
            catch(Exception ex)
            {
                resposta.Mensagem = ex.Message;
                resposta.Status = false;
                return resposta;

            }
        }

        public async Task<ResponseModel<List<ClienteModel>>> ListarClientes()
        {
            ResponseModel<List<ClienteModel>> resposta = new ResponseModel<List<ClienteModel>>();
            try
            {
                var clientes = await _context.Clientes.ToListAsync();

                resposta.Dados = clientes;
                resposta.Mensagem = " Todos clientes foram coletados!";

                return resposta;
            }
            catch(Exception ex)
            {
                resposta.Mensagem = ex.Message;
                resposta.Status = false;
                return resposta;
            }
        }
    }
}
