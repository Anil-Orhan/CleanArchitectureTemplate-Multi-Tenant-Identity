namespace CleanArcBase.Application.Common.Models;

public class AuthResult
{
    public bool Succeeded { get; private set; }
    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public string? Error { get; private set; }

    public static AuthResult Success(string accessToken, string refreshToken, DateTime expiresAt)
        => new()
        {
            Succeeded = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt
        };

    public static AuthResult Failure(string error)
        => new()
        {
            Succeeded = false,
            Error = error
        };
}

public class TokenResult
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
