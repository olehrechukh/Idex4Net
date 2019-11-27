namespace Idex4Net.Domain.ExchangeModels
{
    public class Parameters
    {
        public string TokenBuy { get; set; }
        public int BuyPrecision { get; set; }
        public string AmountBuy { get; set; }
        public string TokenSell { get; set; }
        public int SellPrecision { get; set; }
        public string AmountSell { get; set; }
        public long Expires { get; set; }
        public long Nonce { get; set; }
        public string User { get; set; }

        public override string ToString()
        {
            return
                $"{nameof(TokenBuy)}: {TokenBuy}, {nameof(BuyPrecision)}: {BuyPrecision}, {nameof(AmountBuy)}: {AmountBuy}, {nameof(TokenSell)}: {TokenSell}, {nameof(SellPrecision)}: {SellPrecision}, {nameof(AmountSell)}: {AmountSell}, {nameof(Expires)}: {Expires}, {nameof(Nonce)}: {Nonce}, {nameof(User)}: {User}";
        }
    }
}