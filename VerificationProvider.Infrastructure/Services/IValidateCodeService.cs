using Microsoft.AspNetCore.Http;
using VerificationProvider.Infrastructure.Models;


namespace VerificationProvider.Infrastructure.Services
{
    public interface IValidateCodeService
    {
        Task<ValidateCodeRequest> UnpackValidateRequestAsync(HttpRequest req);
        Task<bool> ValidateCodeAsync(ValidateCodeRequest validateRequest);
    }
}