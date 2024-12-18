using Application.Response;
using MediatR;

namespace Application.MediatrPipeline;

public class ExceptionHandlingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await next();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return (TResponse)Activator.CreateInstance(typeof(TResponse), new object[]
                {
                    e.Message,
                    //"Server error", 
                    Status.ServerError, 
                    false
                }
            );
        }
    }
}
