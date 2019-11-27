namespace Idex4Net.Domain.ExchangeModels
{
    public class IdexNextNonce
    {
        public long Nonce;

        public override string ToString()
        {
            return $"{nameof(Nonce)}: {Nonce}";
        }
    }
}