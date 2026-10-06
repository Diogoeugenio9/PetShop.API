namespace PetShop.API.Utils
{
    public class ConflitoException : Exception
    {
        public ConflitoException(string mensagem) : base(mensagem)
        {
        }
    }
}
