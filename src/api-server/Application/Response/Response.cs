using System.Text.Json.Serialization;

namespace Application.Response;

public class ResponseBase
{
    public bool Successful { get; set; }
    public string Message { get; set; }
    public Status Status { get; set; }
    
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

    [JsonConstructor] // for deserialization
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
    public static Response<T> Failure(string message, Status status)
    {
        return new Response<T>(message, status, false);
    }
}