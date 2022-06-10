namespace CymaxStore.API.Models
{
    public class Result
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }

        public Result()
        {
        }

        public Result(string message = null)
        {
            Succeeded = true;
            Message = message;
        }

        public static Result Success(string message = null)
        {
            return new Result { Succeeded = true, Message = message };
        }

        public static Result Failure(string message, List<string> errors = null)
        {
            return new Result { Message = message, Errors = errors };
        }
    }

    public class Result<T> : Result
    {
        public T Data { get; set; }

        public Result()
        {
        }

        public static Result<T> Success(T data, string message = null)
        {
            return new Result<T> { Succeeded = true, Data = data, Message = message };
        }

        public static Result<T> Failure(T data, string message = null, List<string> errors = null)
        {
            return new Result<T> { Data = data, Message = message, Errors = errors };
        }

        public static new Result<T> Failure(string message, List<string> errors = null)
        {
            return new Result<T> { Message = message, Errors = errors };
        }

        public Result(T data, string message = null)
        {
            Succeeded = true;
            Message = message;
            Data = data;
        }
    }

    public class PagedResult<T> : Result
    {
        public IEnumerable<T> Data { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public long TotalCount { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        public static PagedResult<T> Success(IEnumerable<T> data, long count, int pageNumber, int pageSize, string message = null)
        {
            return new PagedResult<T>
            {
                Succeeded = true,
                Data = data,
                TotalCount = count,
                Page = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                Message = message ?? "Items retrieved okay"
            };
        }

        public static PagedResult<T> Failure(string message = null)
        {
            return new PagedResult<T>
            {
                Succeeded = false,
                Message = message ?? "Unable to retrieve items"
            };
        }
    }
}