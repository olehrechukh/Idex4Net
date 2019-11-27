namespace Idex4Net.Domain.CoreModels
{
    public readonly struct CurrencyPair
    {
        public readonly string Base;
        public readonly string Quote;

        public CurrencyPair(string @base, string quote)
        {
            Base = @base;
            Quote = quote;
        }

        public static CurrencyPair From(string @base, string quote)
        {
            return new CurrencyPair(@base, quote);
        }

        public override string ToString()
        {
            return $"{nameof(Base)}: {Base}, {nameof(Quote)}: {Quote}";
        }
    }
}