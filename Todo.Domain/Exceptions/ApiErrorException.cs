using System.Text.Json.Serialization;

namespace Todo.Domain.Exceptions;

[Serializable]
public class ApiErrorException : Exception
{
    public int StatusCode { get; }
    public ApiErrorException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;

    }
}
