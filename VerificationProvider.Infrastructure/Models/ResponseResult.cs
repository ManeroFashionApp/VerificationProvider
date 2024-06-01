using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificationProvider.Infrastructure.Models
{
    public class ResponseResult<T>
    {
        public bool IsSuccess { get; set; }
        public string? Error { get; set; }
        public T? Result { get; set; }
    }
}
