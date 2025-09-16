using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Common.Extensions
{
    public class BaseActionResult<TData> : IConvertToActionResult
    {
        private const int DefaultStatusCode = StatusCodes.Status200OK;

        public bool? Success { get; set; }

        public string? Message { get; set; }

        public TData? Data { get; }

        public ActionResult? Result;

        public BaseActionResult(TData data, string? message = "Message", bool? success = true)
        {
            Data = data;
            Message = message;  
            Success = success;
        }

        public BaseActionResult(ActionResult result, string? message = "Message", bool? success = true)
        {
            Result = result;
            Message = message;
            Success = success;
        }

        public static implicit operator BaseActionResult<TData>(TData value) => new(value);

        public static implicit operator BaseActionResult<TData>(ActionResult result) => new(result);

        public IActionResult Convert()
        {
            if (Result != null)
            {
                return Result;
            }

            int statusCode;
            if (Data is ProblemDetails problemDetails && problemDetails.Status != null)
            {
                statusCode = problemDetails.Status.Value;
            }
            else
            {
                statusCode = DefaultStatusCode;
            }

            return new ObjectResult(Data)
            {
                DeclaredType = typeof(TData),
                StatusCode = statusCode
            };
        }
    }
}
