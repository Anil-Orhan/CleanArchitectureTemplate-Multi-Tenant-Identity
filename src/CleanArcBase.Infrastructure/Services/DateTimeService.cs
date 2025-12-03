using CleanArcBase.Application.Common.Interfaces;

namespace CleanArcBase.Infrastructure.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}
