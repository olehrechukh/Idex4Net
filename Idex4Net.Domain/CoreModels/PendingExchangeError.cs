using System;

namespace Idex4Net.Domain.CoreModels
{
    public class PendingExchangeError : ExchangeError
    {
        public TimeSpan PendingTime { get; }

        public PendingExchangeError(TimeSpan pendingTime, string message, long? code = null)
            : base(ExchangeErrorType.PendingError, message)
        {
            PendingTime = pendingTime;
        }
    }
}