using System.Text.Json.Serialization;

namespace Todo.Domain.Exceptions;

[Serializable]
public class ApiErrorException : Exception
{
    public IList<string>? Errors { get; set; }
    public int StatusCode { get; }

    public ApiErrorException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;

    }
    public ApiErrorException(string message, int statusCode, IList<string>? errors) : this(message, statusCode)
    {
        Errors = errors;
    }
}
