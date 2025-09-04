namespace ScrummersSalesApi.Backend.Products.Domain.Dto.Products
{

    public sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int Total { get; init; }
    }
}
