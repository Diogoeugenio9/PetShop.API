namespace PetShop.API.Utils
{
    public class AcessoNegadoException : Exception
    {
        public AcessoNegadoException(string mensagem) : base(mensagem)
        {
        }
    }
}
