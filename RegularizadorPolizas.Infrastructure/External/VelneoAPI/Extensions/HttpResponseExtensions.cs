namespace RegularizadorPolizas.Infrastructure.External.VelneoAPI.Extensions
{
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// Extrae el total count de los headers de respuesta de Velneo API
        /// </summary>
        /// <param name="response">HttpResponseMessage de Velneo API</param>
        /// <returns>Total count si está presente en headers, null si no</returns>
        public static int? GetTotalCountFromHeaders(this HttpResponseMessage response)
        {
            if (response == null) return null;

            // ✅ Headers comunes de paginación en APIs REST
            var possibleHeaders = new[]
            {
                "X-Total-Count",      // Header estándar
                "X-Total",            // Variante común
                "Total-Count",        // Sin prefijo X-
                "Total",              // Más simple
                "X-Pagination-Total", // Más específico
                "X-Page-Total"        // Específico de página
            };

            foreach (var headerName in possibleHeaders)
            {
                if (response.Headers.TryGetValues(headerName, out var values))
                {
                    var firstValue = values.FirstOrDefault();
                    if (!string.IsNullOrEmpty(firstValue) && int.TryParse(firstValue, out var totalCount))
                    {
                        return totalCount;
                    }
                }
            }

            // ✅ También intentar en Content-Range header (formato HTTP estándar)
            if (response.Content?.Headers != null &&
                response.Content.Headers.TryGetValues("Content-Range", out var contentRangeValues))
            {
                var contentRange = contentRangeValues.FirstOrDefault();
                if (!string.IsNullOrEmpty(contentRange))
                {
                    // Format: "items 0-24/100" donde 100 es el total
                    var parts = contentRange.Split('/');
                    if (parts.Length == 2 && int.TryParse(parts[1], out var total))
                    {
                        return total;
                    }
                }
            }

            // ✅ No se encontró total count en headers
            return null;
        }

        /// <summary>
        /// Extrae información de paginación completa de los headers
        /// </summary>
        /// <param name="response">HttpResponseMessage de Velneo API</param>
        /// <returns>Información de paginación extraída</returns>
        public static PaginationInfo GetPaginationInfo(this HttpResponseMessage response)
        {
            return new PaginationInfo
            {
                TotalCount = response.GetTotalCountFromHeaders(),
                CurrentPage = GetHeaderValue(response, "X-Current-Page", "X-Page-Number"),
                PageSize = GetHeaderValue(response, "X-Page-Size", "X-Per-Page"),
                TotalPages = GetHeaderValue(response, "X-Total-Pages", "X-Page-Count"),
                HasNext = GetBooleanHeaderValue(response, "X-Has-Next", "X-Has-More"),
                HasPrevious = GetBooleanHeaderValue(response, "X-Has-Previous", "X-Has-Prev")
            };
        }

        private static int? GetHeaderValue(HttpResponseMessage response, params string[] headerNames)
        {
            foreach (var headerName in headerNames)
            {
                if (response.Headers.TryGetValues(headerName, out var values))
                {
                    var firstValue = values.FirstOrDefault();
                    if (!string.IsNullOrEmpty(firstValue) && int.TryParse(firstValue, out var intValue))
                    {
                        return intValue;
                    }
                }
            }
            return null;
        }

        private static bool? GetBooleanHeaderValue(HttpResponseMessage response, params string[] headerNames)
        {
            foreach (var headerName in headerNames)
            {
                if (response.Headers.TryGetValues(headerName, out var values))
                {
                    var firstValue = values.FirstOrDefault();
                    if (!string.IsNullOrEmpty(firstValue) && bool.TryParse(firstValue, out var boolValue))
                    {
                        return boolValue;
                    }
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Información de paginación extraída de headers HTTP
    /// </summary>
    public class PaginationInfo
    {
        public int? TotalCount { get; set; }
        public int? CurrentPage { get; set; }
        public int? PageSize { get; set; }
        public int? TotalPages { get; set; }
        public bool? HasNext { get; set; }
        public bool? HasPrevious { get; set; }
    }
}