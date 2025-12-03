using CleanArcBase.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArcBase.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Only wrap commands in transactions (not queries)
        var isCommand = typeof(TRequest).GetInterfaces()
            .Any(i => i.IsGenericType &&
                     (i.GetGenericTypeDefinition() == typeof(ICommand<>) ||
                      i == typeof(ICommand)));

        if (!isCommand)
        {
            return await next();
        }

        var requestName = typeof(TRequest).Name;

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            _logger.LogDebug("Beginning transaction for {RequestName}", requestName);

            var response = await next();

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            _logger.LogDebug("Committed transaction for {RequestName}", requestName);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing {RequestName}, rolling back transaction", requestName);
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
