using AutoFixture;
using AutoFixture.Xunit2;

namespace Idex4Net.Tests
{
    public class Conventions : AutoDataAttribute
    {
        private const string ApiKey = "ApiKey";
        private const string ApiSecret = "0xApiSecret";
        private const string WalletAddress = "0xWalletAddress";

        public Conventions() : base(Create)
        {
        }

        private static IFixture Create()
        {
            var fixture = new Fixture();

            var apiCredentials = new ApiCredentials(ApiKey, ApiSecret, WalletAddress);

            fixture.Inject(new IdexClient(apiCredentials));

            return fixture;
        }
    }
}