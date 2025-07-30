namespace RegularizadorPolizas.Application.DTOs.Velneo
{
    public class PaginatedVelneoResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;
        public int CurrentPage => PageNumber; 
        public int VelneoTotalPages { get; set; }
        public bool VelneoHasMoreData { get; set; }
        public TimeSpan RequestDuration { get; set; }
        public DateTime RetrievedAt { get; set; } = DateTime.UtcNow;
        public string DataSource { get; set; } = "velneo_paginated";
        public bool IsEstimatedTotal { get; set; } = false;
        public int ActualItemsCount => Items?.Count() ?? 0;
        public bool IsEmpty => ActualItemsCount == 0;
        public bool IsFullPage => ActualItemsCount == PageSize;
        public double RequestDurationMs => RequestDuration.TotalMilliseconds;
    }

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

    public class VelneoServiceConfig
    {
        public int DefaultPageSize { get; set; } = 50;
        public int MaxPageSize { get; set; } = 1000;
        public int TimeoutSeconds { get; set; } = 300;
        public bool EnableRetry { get; set; } = true;
        public int MaxRetryAttempts { get; set; } = 3;
    }
}