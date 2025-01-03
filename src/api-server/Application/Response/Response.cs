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
    Ok,      // 200
    NoContent,    // 204
    BadRequest,   // 400
    NotFound,     // 404
    ServerError   // 500
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

    public static Response<T> Ok(string message, T data) => 
        new Response<T>(message, Status.Ok, true, data);
    public static Response<T> NoContent() => 
        new Response<T>("", Status.NoContent, true);
    public static Response<T> BadRequest(string message) =>
        new Response<T>(message, Status.BadRequest, false);
    public static Response<T> NotFound(string message) =>
        new Response<T>(message, Status.NotFound, false);
    public static Response<T> ServerError(string message) =>
         new Response<T>(message, Status.ServerError, false);
}