using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Application;
using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

// using Microsoft.EntityFrameworkCore;
// using Npgsql;

namespace Presentation.ExceptionHandler;

public class ProblemExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;

    public ProblemExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;
        
        Console.WriteLine($"Exception type: {exception.GetType().Name}");
        
        if (exception is ProblemException problemException)
        {
            Console.WriteLine("ProblemException caught.");
            
            problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = problemException.Title,
                Detail = problemException.Message,
                Type = "Bad Request"
            };
        }
        else if (exception is DbUpdateException)
        {
            Console.WriteLine("Handling DbUpdateException...");

            problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "Bad Request"
            };
            
            switch (exception)
            {
                case UniqueConstraintException:
                    problemDetails.Title = "Unique Constraint Violation";
                    problemDetails.Detail = "Tried to create/update an entity that has a duplicate key.";
                    break;
                case CannotInsertNullException:
                    problemDetails.Title = "Cannot Insert Null Entity";
                    problemDetails.Detail = "Tried to create/update an entity with a null value that is not allowed.";
                    break;
                case MaxLengthExceededException:
                    problemDetails.Title = "Field Max Length Exceeded";
                    problemDetails.Detail = "Tried to create/update an entity with a field that is too long.";
                    break;
                case NumericOverflowException:
                    problemDetails.Title = "Numeric Overflow Exception";
                    problemDetails.Detail = "";
                    break;
                case ReferenceConstraintException:
                    problemDetails.Title = "Reference Constraint Violation";
                    problemDetails.Detail = "Tried to create/update an entity that has an invalid foreign key.";
                    break;
                case ValidationException:
                    problemDetails.Title = "Validation Exception";
                    problemDetails.Detail = "";
                    break;
                default:
                    throw new UnreachableException();
                    break;
            }
        }
        else
        {
            Console.WriteLine("Unknown exception.");
            
            problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Unknown Error",
                Detail = "Try something else.",
                Type = "Server Error"
            };
        }
        
        httpContext.Response.StatusCode = problemDetails.Status.Value;
        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails
            });
    }
    
    // private Dictionary<string, string> ExtractConstraintDetails(PostgresException postgresEx)
    // {
    //     // Example of parsing the details for constraint information
    //     var details = new Dictionary<string, string>();
    //
    //     // Typically, the `Detail` field might look like:
    //     // "Key (parent_id)=(1234) is not present in table "parent_table"."
    //     if (!string.IsNullOrEmpty(postgresEx.Detail))
    //     {
    //         Console.WriteLine($"Detail: {postgresEx.Detail}");
    //         var match = Regex.Match(postgresEx.Detail, @"Key \((?<field>\w+)\)=\((?<value>.+?)\)");
    //         if (match.Success)
    //         {
    //             details["field"] = match.Groups["field"].Value;
    //             details["invalid_value"] = match.Groups["value"].Value;
    //             Console.WriteLine($"[{match.Groups["field"].Value}]: {match.Groups["value"].Value}");
    //         }
    //     }
    //
    //     return details;
    // }
}