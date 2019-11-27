namespace Idex4Net
{
    public readonly struct ApiCredentials
    {
        public readonly string ApiKey;
        public readonly string ApiSecret;
        public readonly string WalletAddress;

        public ApiCredentials(string apiKey, string apiSecret, string walletAddress)
        {
            ApiSecret = apiSecret;
            WalletAddress = walletAddress;
            ApiKey = apiKey;
        }
    }
}