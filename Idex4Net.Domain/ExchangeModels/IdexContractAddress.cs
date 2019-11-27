namespace Idex4Net.Domain.ExchangeModels
{
    public class IdexContractAddress
    {
        public string Address;

        public override string ToString()
        {
            return $"{nameof(Address)}: {Address}";
        }
    }
}