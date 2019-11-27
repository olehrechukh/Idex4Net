using System;

namespace Idex4Net.Domain.CoreModels
{
    public class ExchangeError
    {
        public ExchangeErrorType Type { get; }
        public string Message { get; }

        public ExchangeError(ExchangeErrorType type, string message)
        {
            Type = type;
            Message = message;
        }

        public override string ToString()
        {
            return $"{nameof(Type)}: {Type}, {nameof(Message)}: {Message}";
        }

        public static PendingExchangeError Pending(TimeSpan timeSpan, string errorMessage)
        {
            return new PendingExchangeError(timeSpan, errorMessage);
        }
    }
}