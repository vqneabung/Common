using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class BaseSuccess
    {
        public string? Message { get; set; }

        public BaseSuccess(string? message = "Success")
        {
            Message = message;
        }

        public static implicit operator BaseSuccess(string message) => new(message);

    }
}
