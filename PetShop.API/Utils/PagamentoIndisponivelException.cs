namespace PetShop.API.Utils
{
    public class PagamentoIndisponivelException : Exception
    {
        public PagamentoIndisponivelException(string mensagem) : base(mensagem)
        {
        }
    }
}
