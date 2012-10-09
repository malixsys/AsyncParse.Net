using System.Collections.Generic;

namespace AsyncParse.Net.Model
{
    public class AsyncCallResult<T>
        where T : class
    {
        private readonly AsyncCallFailureReason _asyncCallFailureReason;

        public AsyncCallResult(AsyncCallFailureReason asyncCallFailureReason_, T result = null)
        {
            _asyncCallFailureReason = asyncCallFailureReason_;
            Contents = result;
        }

        public T Contents { get; private set; }

        public AsyncCallResult()
        {
        }

        public AsyncCallFailureReason FailureReason
        {
            get { return _asyncCallFailureReason; }
        }
    }

    public enum AsyncCallFailureReason
    {
        None,
        TimeOut,
        ResultsNotFound,
        FailedStatusCode,
        FailedConnection
    }

    public class DeleteResponse
    {
        public bool Success { get; set; }
    }

    public class GetListResponse<T>
    {
        public List<T> Results;

        public int Count
        {
            get { return Results == null ? 0 : Results.Count; }
        }
        public T this[int i_]
        {
            get { return Results == null || Results.Count <= i_ ? default(T) : Results[i_]; }
        }
    }
}