using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Domain.Shared.Wrapper
{
    public class Response<T>
    {
        public T Result { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public string MessageCode { get; set; }
        public List<string> BrokenRules { get; set; }

        public Response() { }

        public Response(T result, bool succeeded = true, string message = "", string messageCode = "")
        {
            Result = result;
            Succeeded = succeeded;
            Message = message;
            MessageCode = messageCode;
        }

        public static Response<T> Success(T data, string message = "", string messageCode = "")
            => new(data, true, message, messageCode);

        public static Response<T> Fail(List<string> errors, string message = "", string messageCode = "")
            => new(default, false, message, messageCode) { BrokenRules = errors };

        public static Response<T> Fail(string error, string message = "", string messageCode = "")
            => Fail(new List<string> { error }, message, messageCode);
    }
}

