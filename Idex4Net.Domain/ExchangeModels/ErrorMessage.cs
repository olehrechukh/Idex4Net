namespace Idex4Net.Domain.ExchangeModels
{
    public class ErrorMessage
    {
        public string Error;

        public override string ToString()
        {
            return $"{nameof(Error)}: {Error}";
        }
    }
}