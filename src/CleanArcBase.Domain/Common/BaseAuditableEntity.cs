namespace CleanArcBase.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; protected set; }
    public string? CreatedBy { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public string? UpdatedBy { get; protected set; }

    public void SetCreatedInfo(string? userId, DateTime createdAt)
    {
        CreatedBy = userId;
        CreatedAt = createdAt;
    }

    public void SetUpdatedInfo(string? userId, DateTime updatedAt)
    {
        UpdatedBy = userId;
        UpdatedAt = updatedAt;
    }
}
