namespace Idex4Net.Domain.CoreModels
{
    public readonly struct Order
    {
        public readonly CurrencyPair CurrencyPair;
        public readonly decimal Price;
        public readonly decimal Amount;
        public readonly OrderSide Side;

        public Order(CurrencyPair currencyPair, decimal price, decimal amount, OrderSide side)
        {
            CurrencyPair = currencyPair;
            Price = price;
            Amount = amount;
            Side = side;
        }

        public override string ToString()
        {
            return
                $"{nameof(CurrencyPair)}: {CurrencyPair}, {nameof(Price)}: {Price}, {nameof(Amount)}: {Amount}, {nameof(Side)}: {Side}";
        }
    }
}