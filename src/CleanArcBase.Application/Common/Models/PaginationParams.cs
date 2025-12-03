namespace CleanArcBase.Application.Common.Models;

public record PaginationParams
{
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 10;

    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = DefaultPageSize;

    public PaginationParams() { }

    public PaginationParams(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize > MaxPageSize ? MaxPageSize : (pageSize < 1 ? DefaultPageSize : pageSize);
    }
}
