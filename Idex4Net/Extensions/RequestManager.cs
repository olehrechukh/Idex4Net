using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Idex4Net.Domain.CoreModels;

namespace Idex4Net.Extensions
{
    public static class RequestManager
    {
        public static async Task<T> WithRetry<T>(
            this Func<Task<Result<T>>> task,
            int maxRequestCount = 10,
            [CallerMemberName] string callerName = null)
        {
            for (var i = 0; i < maxRequestCount; i++)
            {
                var result = await task();

                if (result.IsSuccess)
                {
                    return result.Data;
                }
            }

            throw new Exception($"Can not request {callerName}");
        }
    }
}