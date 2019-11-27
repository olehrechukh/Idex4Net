namespace Idex4Net.Domain.CoreModels
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Data { get; }
        public ExchangeError ErrorData { get; }

        private Result(bool isSuccess, ExchangeError errorData, T data)
        {
            Data = data;
            IsSuccess = isSuccess;
            ErrorData = errorData;
        }

        public static Result<T> Failed(ExchangeError error)
        {
            return new Result<T>(false, error, default);
        }

        public static Result<T> Success(T data)
        {
            return new Result<T>(true, default, data);
        }
    }
}