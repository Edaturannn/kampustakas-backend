namespace Takas.Api.Helpers;

public class AppException : Exception
{
    public AppException(string message, int statusCode, IEnumerable<string>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors?.ToList() ?? new List<string>();
    }

    public int StatusCode { get; }
    public List<string> Errors { get; }
}

public sealed class BadRequestException : AppException
{
    public BadRequestException(string message, IEnumerable<string>? errors = null)
        : base(message, StatusCodes.Status400BadRequest, errors)
    {
    }
}

public sealed class UnauthorizedAppException : AppException
{
    public UnauthorizedAppException(string message)
        : base(message, StatusCodes.Status401Unauthorized)
    {
    }
}

public sealed class ForbiddenException : AppException
{
    public ForbiddenException(string message)
        : base(message, StatusCodes.Status403Forbidden)
    {
    }
}

public sealed class NotFoundException : AppException
{
    public NotFoundException(string message)
        : base(message, StatusCodes.Status404NotFound)
    {
    }
}

public sealed class ConflictException : AppException
{
    public ConflictException(string message)
        : base(message, StatusCodes.Status409Conflict)
    {
    }
}
