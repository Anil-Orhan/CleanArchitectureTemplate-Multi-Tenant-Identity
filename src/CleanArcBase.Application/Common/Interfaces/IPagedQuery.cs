using CleanArcBase.Application.Common.Models;
using MediatR;

namespace CleanArcBase.Application.Common.Interfaces;

/// <summary>
/// Marker interface for paginated queries.
/// Any query implementing this interface will automatically support pagination.
/// </summary>
public interface IPagedQuery<TResponse> : IRequest<PagedResponse<TResponse>>
{
    int PageNumber { get; }
    int PageSize { get; }
}
