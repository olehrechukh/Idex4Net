namespace Idex4Net.Domain.CoreModels
{
    public enum ExchangeErrorType
    {
        Unknown,
        OrderNotFound,
        InsufficientFunds,
        InvalidOrder,
        Authentication,
        ParsingError,
        PendingError
    }
}