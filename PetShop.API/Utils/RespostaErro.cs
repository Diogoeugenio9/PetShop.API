namespace PetShop.API.Utils
{
    public class RespostaErro
    {
        public RespostaErro(string message)
        {
            Message = message;
        }

        public string Message { get; }
        public string Mensagem => Message;
    }
}
