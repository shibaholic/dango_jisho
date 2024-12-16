using System.Text.Json.Serialization;

namespace Application.Response;

public class ResponseBase
{
    public bool Successful { get; }
    public string Message { get; }
    public Status Status { get; }
    
    [JsonConstructor] // for deserialization
    public ResponseBase(string message, Status status, bool successful)
    {
        Message = message;
        Status = status;
        Successful = successful;
    }
    public ResponseBase()
    {
        
    }
}

public enum Status
{
    Success,
    NotFound,
    ServerError
}

public class Response<T> : ResponseBase
{
    public T? Data { get; set; }

    [JsonConstructor]
    private Response(string message, Status status, bool successful, T? data=default): base(message, status, successful)
    {
        Data = data;
    }

    public Response(string message, Status status, bool successful) : base(message, status, successful)
    {
        
    }

    public static Response<T> Success(string message, T? data = default)
    {
        return new Response<T>(message, Status.Success, true, data);
    }

    public static Response<T> NotFound(string message)
    {
        return new Response<T>(message, Status.NotFound, true);
    }
    public static Response<T> Error(string message, Status status)
    {
        return new Response<T>(message, status, false);
    }
}