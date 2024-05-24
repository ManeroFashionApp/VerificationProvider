using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerificationProvider.Models
{
    public class ValidateCodeRequest
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
