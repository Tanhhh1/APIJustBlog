namespace Application.Common.ModelServices
{
    public class ApiResult<T>
    {
        public bool Succeeded { get; set; } = false;
        public T? Result { get; set; }
        public IEnumerable<string> Errors { get; set; } = Array.Empty<string>();
        public ApiResult() { }
        public ApiResult(T result, IEnumerable<string> errors)
        {
            Result = result;
            Errors = errors;
        }
        public ApiResult(bool succeeded, T result, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Result = result;
            Errors = errors;
        }
        public static ApiResult<T> Success(T result)
        {
            return new ApiResult<T>(true, result, new List<string>());
        }
        public static ApiResult<T?> Failure(IEnumerable<string> errors)
        {
            return new ApiResult<T?>(false, default, errors);
        }
    }
}
