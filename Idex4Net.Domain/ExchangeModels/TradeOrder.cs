using System;

namespace Idex4Net.Domain.ExchangeModels
{
    public class TradeOrder
    {
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public string Market { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public string OrderHash { get; set; }
        public string Uuid { get; set; }

        public override string ToString()
        {
            return $"{nameof(Amount)}: {Amount}, {nameof(Date)}: {Date}, {nameof(Total)}: {Total}, {nameof(Market)}: {Market}, {nameof(Type)}: {Type}, {nameof(Price)}: {Price}, {nameof(OrderHash)}: {OrderHash}, {nameof(Uuid)}: {Uuid}";
        }
    }
}