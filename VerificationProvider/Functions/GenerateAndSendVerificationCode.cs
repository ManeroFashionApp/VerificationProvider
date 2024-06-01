using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using VerificationProvider.Infrastructure.Services;


namespace VerificationProvider.Functions
{
    public class GenerateAndSendVerificationCode
    {
        private readonly ILogger<GenerateAndSendVerificationCode> _logger;
        private readonly IVerificationService _verificationService;

        public GenerateAndSendVerificationCode(ILogger<GenerateAndSendVerificationCode> logger, IVerificationService verificationService)
        {
            _logger = logger;
            _verificationService = verificationService;
        }

        [Function(nameof(GenerateAndSendVerificationCode))]
        [ServiceBusOutput("email_request", Connection = "ServiceBusConnectionEmail")]
        public async Task<string> Run(
            [ServiceBusTrigger("verification", Connection = "ServiceBusConnectionVerification")]
            ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Function triggered");
            try
            {
                var verificationRequest = _verificationService.UnpackVerificationRequest(message);
                if (verificationRequest != null)
                {
                    var code = _verificationService.GenerateCode();
                    if (!string.IsNullOrEmpty(code))
                    {
                        var result = await _verificationService.SaveVerificationRequest(verificationRequest, code);
                        if (result.Result)
                        {
                            var emailRequest = _verificationService.GenerateEmailRequest(verificationRequest, code);
                            if (emailRequest != null)
                            {
                                var payload = _verificationService.GenerateServiceBusEmailRequest(emailRequest);
                                if (!string.IsNullOrEmpty(payload))
                                {
                                    await messageActions.CompleteMessageAsync(message);
                                    _logger.LogInformation($"Skickade vidare meddelandet till email_request: {payload}");

                                    return payload;

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: GenerateAndSendVerificationCode.Run() :: {ex.Message}");

            }
            return null!;
        }
    }
}
