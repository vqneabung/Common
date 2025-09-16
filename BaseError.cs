using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class BaseError
    {
        public string? Message { get; set; }

        public object? Errors { get; set; }

        public BaseError(string? message = "Error", object? errors = null)
        {
            Message = message;
            Errors = errors;
        }

        public static implicit operator BaseError(string message) => new(message);

        public static implicit operator BaseError((string message, object errors) tuple) => new(tuple.message, tuple.errors);


    }
}
