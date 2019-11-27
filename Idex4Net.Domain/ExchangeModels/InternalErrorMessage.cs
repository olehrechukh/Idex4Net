namespace Idex4Net.Domain.ExchangeModels
{
    public class InternalErrorMessage
    {
        public string Message;

        public override string ToString()
        {
            return $"{nameof(Message)}: {Message}";
        }
    }
}