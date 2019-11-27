namespace Idex4Net.Domain.ExchangeModels
{
    public class Balance
    {
        public decimal Available;
        public decimal OnOrders;

        public override string ToString()
        {
            return $"{nameof(Available)}: {Available}, {nameof(OnOrders)}: {OnOrders}";
        }
    }
}