namespace RegularizadorPolizas.Application.DTOs
{
    public class BatchOperationResult<T>
    {
        public int TotalRequested { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public List<T> SuccessfulItems { get; set; } = new();
        public List<BatchOperationError> Errors { get; set; } = new();
        public TimeSpan Duration { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

        public double SuccessRate => TotalRequested > 0 ? (double)SuccessCount / TotalRequested * 100 : 0;
        public bool AllSucceeded => ErrorCount == 0;
    }

    public class BatchOperationError
    {
        public int Index { get; set; }
        public string Error { get; set; } = string.Empty;
        public object? Item { get; set; }
        public string? ErrorCode { get; set; }
    }

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;
    }
}