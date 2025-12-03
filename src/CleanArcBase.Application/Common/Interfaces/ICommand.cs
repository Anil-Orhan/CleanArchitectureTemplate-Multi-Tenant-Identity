using MediatR;

namespace CleanArcBase.Application.Common.Interfaces;

/// <summary>
/// Marker interface for commands that should be wrapped in a transaction
/// </summary>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

/// <summary>
/// Marker interface for commands without response that should be wrapped in a transaction
/// </summary>
public interface ICommand : IRequest
{
}
