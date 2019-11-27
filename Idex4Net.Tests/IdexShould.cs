using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Idex4Net.Domain.CoreModels;
using Xunit;

namespace Idex4Net.Tests
{
    // Before test running set your credentials in Conventions.cs
    public class IdexShould
    {
        [Theory, Conventions]
        public async Task ReturnCurrencies(IdexClient idexClient)
        {
            var currencies = await idexClient.GetCurrencies(CancellationToken.None);

            currencies.IsSuccess.Should().BeTrue();
            currencies.Data.Should().NotBeEmpty();
        }

        [Theory, Conventions]
        public async Task ReturnBalances(IdexClient idexClient)
        {
            var currencies = await idexClient.GetBalances(CancellationToken.None);

            currencies.IsSuccess.Should().BeTrue();
            currencies.Data.Should().NotBeEmpty();
        }

        [Theory, Conventions]
        public async Task ReturnContractAddress(IdexClient idexClient)
        {
            var currencies = await idexClient.GetContractAddress(CancellationToken.None);

            currencies.IsSuccess.Should().BeTrue();
            currencies.Data.Should().NotBeNull();
        }

        [Theory, Conventions]
        public async Task CreateOrder(IdexClient idexClient)
        {
            var qntEth = CurrencyPair.From("QNT", "ETH");
            var order = new Order(qntEth, 0.0315m, 4.762m, OrderSide.Buy);
            var currencies = await idexClient.CreateOrder(order, CancellationToken.None);

            currencies.IsSuccess.Should().BeTrue();
        }
    }
}