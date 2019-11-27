using System;

namespace Idex4Net.Domain.CoreModels
{
    public class Currency
    {
        public int Decimals { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public decimal DecimalsPower10 => (decimal) Math.Pow(10, Decimals);

        public override string ToString()
        {
            return $"{nameof(Decimals)}: {Decimals}, {nameof(Address)}: {Address}, {nameof(Name)}: {Name}";
        }
    }
}