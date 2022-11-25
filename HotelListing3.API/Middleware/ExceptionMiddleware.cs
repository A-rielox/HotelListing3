using Azure;
using HotelListing3.API.Exceptions;
using Newtonsoft.Json;
using System.Net;

namespace HotelListing3.API.Middleware;

public class ExceptionMiddleware
{
    // cada vez q llegue un request a la api va a pasar por este "next", porque
    // voy a poner eesto como middleware
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    public ExceptionMiddleware(RequestDelegate next,ILogger<ExceptionMiddleware> logger)
    {
        // este next es el request que llega a traves de este middleware
        this._next = next;
        this._logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // este va a ser el try-catch universal, si cualquier req tira una exception
        // aqui lo agarra y maneja
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Something went wrong while " +
                $"processing {context.Request.Path}");

            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        // para especificar q intento responder en json
        context.Response.ContentType = "application/json";
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

        // lo inicializo con valores default para .InternalServerError
        var errorDetails = new ErrorDetails
        {
            ErrorType = "Failure",
            ErrorMessage = ex.Message,
        };

        switch (ex)
        {
            case NotFoundException notFoundException:
                statusCode = HttpStatusCode.NotFound;
                errorDetails.ErrorType = "Not Found";
                break;

            default:
                break;
        }

        string response = JsonConvert.SerializeObject(errorDetails);
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(response);
    }
}

// respuesta q manda en swagger

//Error: response status is 404

//Response body
//Download
//{
//  "ErrorType": "Not Found",
//  "ErrorMessage": "GetCountry (69) was not found"
//}

public class ErrorDetails
{
    public string ErrorType { get; set; }
    public string ErrorMessage { get; set; }
}
