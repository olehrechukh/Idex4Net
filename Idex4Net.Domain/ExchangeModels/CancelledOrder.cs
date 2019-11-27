namespace Idex4Net.Domain.ExchangeModels
{
    public class CancelledOrder
    {
        public int Success;

        public override string ToString()
        {
            return $"{nameof(Success)}: {Success}";
        }
    }
}