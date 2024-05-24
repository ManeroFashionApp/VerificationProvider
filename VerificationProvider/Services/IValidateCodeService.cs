using Microsoft.AspNetCore.Http;
using VerificationProvider.Models;

namespace VerificationProvider.Services
{
    public interface IValidateCodeService
    {
        Task<ValidateCodeRequest> UnpackValidateRequestAsync(HttpRequest req);
        Task<bool> ValidateCodeAsync(ValidateCodeRequest validateRequest);
    }
}