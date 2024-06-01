using Azure.Messaging.ServiceBus;
using VerificationProvider.Infrastructure.Models;


namespace VerificationProvider.Infrastructure.Services
{
    public interface IVerificationService
    {
        string GenerateCode();
        EmailRequest GenerateEmailRequest(VerificationRequest verificationRequest, string code);
        string GenerateServiceBusEmailRequest(EmailRequest emailRequest);
        Task<ResponseResult<bool>> SaveVerificationRequest(VerificationRequest verificationRequest, string code);
        VerificationRequest UnpackVerificationRequest(ServiceBusReceivedMessage message);
    }
}