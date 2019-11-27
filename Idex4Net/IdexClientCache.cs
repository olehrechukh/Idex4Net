using System.Collections.Generic;
using Idex4Net.Domain.CoreModels;
using Idex4Net.Domain.ExchangeModels;

namespace Idex4Net
{
    public class IdexClientCache
    {
        public long? Nonce { get; set; }
        public Dictionary<string, Currency> Currencies { get; set; }
        public string ContractAddress { get; set; }
    }
}