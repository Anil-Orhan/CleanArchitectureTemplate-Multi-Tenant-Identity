using CleanArcBase.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanArcBase.Application.Common.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResponse<T>.Create(items, totalCount, pageNumber, pageSize);
    }

    public static async Task<PagedResponse<TResult>> ToPagedResponseAsync<TSource, TResult>(
        this IQueryable<TSource> query,
        int pageNumber,
        int pageSize,
        Func<TSource, TResult> mapper,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PagedResponse<TResult>.Create(items.Select(mapper).ToList(), totalCount, pageNumber, pageSize);
    }
}
