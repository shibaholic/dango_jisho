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
    Unauthorized, // 401
    NotFound,     // 404
    ServerError   // 500
}

public class Response<T> : ResponseBase
{
    public T? Data { get; set; }
    public int? PageIndex { get; set; } = null;
    public int? PageSize { get; set; } = null;
    public int? ResultCount { get; set; } = null;
    public int? TotalElements { get; set; } = null;

    [JsonConstructor]
    protected Response(string message, Status status, bool successful, 
        T? data=default, int? pageIndex=default, int? pageSize=default, int? resultCount=default, int? totalElements=default
        ) : base(message, status, successful)
    {
        Data = data;
        PageIndex = pageIndex;
        PageSize = pageSize;
        ResultCount = resultCount;
        TotalElements = totalElements;
    }

    private Response(string message, Status status, bool successful) : base(message, status, successful)
    {
        
    }

    public static Response<T> Ok(string message, T data) => 
        new Response<T>(message, Status.Ok, true, data);
    public static Response<T> OkPaginated(string message, T data, int pageIndex, int pageSize, int elementsFound, int totalElements) => 
        new Response<T>(message, Status.Ok, true, data, pageIndex, pageSize, elementsFound, totalElements);
    public static Response<T> NoContent() => 
        new Response<T>("", Status.NoContent, true);
    public static Response<T> BadRequest(string message) =>
        new Response<T>(message, Status.BadRequest, false);
    public static Response<T> Unauthorized(string message) => 
        new Response<T>(message, Status.Unauthorized, false); 
    public static Response<T> NotFound(string message) =>
        new Response<T>(message, Status.NotFound, false);
    public static Response<T> ServerError(string message) {
        Console.Error.WriteLine(message);
        return new Response<T>(message, Status.ServerError, false);
    }
}