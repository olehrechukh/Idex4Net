using Newtonsoft.Json;

namespace Idex4Net.Domain.ExchangeModels
{
    public class CreatedOrder
    {
        public string Error { get; set; }
        public long OrderNumber { get; set; }
        public string OrderHash { get; set; }
        public long Timestamp { get; set; }
        public string Market { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public decimal Total { get; set; }
        public string Type { get; set; }

        [JsonProperty("params")]
        public Parameters Parameters { get; set; }

        public override string ToString()
        {
            return
                $"{nameof(Error)}: {Error}, {nameof(OrderNumber)}: {OrderNumber}, {nameof(OrderHash)}: {OrderHash}, {nameof(Timestamp)}: {Timestamp}, {nameof(Market)}: {Market}, {nameof(Price)}: {Price}, {nameof(Amount)}: {Amount}, {nameof(Total)}: {Total}, {nameof(Type)}: {Type}, {nameof(Parameters)}: {Parameters}";
        }
    }
}