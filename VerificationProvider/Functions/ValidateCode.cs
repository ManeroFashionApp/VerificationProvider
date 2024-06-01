using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VerificationProvider.Infrastructure.Services;

namespace VerificationProvider.Functions
{
    public class ValidateCode
    {
        private readonly ILogger<ValidateCode> _logger;
        private readonly IValidateCodeService _validateCodeService;

        public ValidateCode(ILogger<ValidateCode> logger, IValidateCodeService validateCodeService)
        {
            _logger = logger;
            _validateCodeService = validateCodeService;
        }

        [Function("ValidateCode")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "validate")] HttpRequest req)
        {
            try
            {
                var validateRequest = await _validateCodeService.UnpackValidateRequestAsync(req);
                if (validateRequest != null)
                {
                    var validateResult = await _validateCodeService.ValidateCodeAsync(validateRequest);
                    if (validateResult)
                    {
                        return new OkResult();
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: ValidateVerificationCode.Run() :: {ex.Message}");

            }
            return new UnauthorizedResult();
        }
    }
}
