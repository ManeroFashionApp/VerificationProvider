using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Data.Contexts;
using VerificationProvider.Models;
using static System.Net.Mime.MediaTypeNames;


namespace VerificationProvider.Services
{
    public class VerificationService : IVerificationService
    {
        private readonly ILogger<VerificationService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public VerificationService(ILogger<VerificationService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }


        public VerificationRequest UnpackVerificationRequest(ServiceBusReceivedMessage message)
        {
            try
            {
                var verificationRequest = JsonConvert.DeserializeObject<VerificationRequest>(message.Body.ToString());
                if (verificationRequest != null && !string.IsNullOrEmpty(verificationRequest.Email))
                    return verificationRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: GenerateVerificationCode.UnpackVerificationRequest() :: {ex.Message}");

            }

            return null!;
        }

        public string GenerateCode()
        {
            try
            {
                var rnd = new Random();
                var code = rnd.Next(100000, 999999);

                return code.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: GenerateVerificationCode.GenerateCode() :: {ex.Message}");

            }

            return null!;
        }

        public async Task<bool> SaveVerificationRequest(VerificationRequest verificationRequest, string code)
        {
            try
            {
                using var context = _serviceProvider.GetRequiredService<DataContext>();
                var existingRequest = await context.IncomingRequests.FirstOrDefaultAsync(x => x.Email == verificationRequest.Email);
                if (existingRequest != null)
                {
                    existingRequest.Code = code;
                    existingRequest.ExpiryDate = DateTime.Now.AddMinutes(5);
                    context.Entry(existingRequest).State = EntityState.Modified;
                }
                else
                {
                    context.IncomingRequests.Add(new Data.Entities.IncomingRequestEntity() { Email = verificationRequest.Email, Code = code });
                }

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: GenerateVerificationCode.SaveVerificationRequest() :: {ex.Message}");

            }

            return false;
        }

        public EmailRequest GenerateEmailRequest(VerificationRequest verificationRequest, string code)
        {
            try
            {
                if (!string.IsNullOrEmpty(verificationRequest.Email) && !string.IsNullOrEmpty(code))
                {
                    var emailRequest = new EmailRequest()
                    {
                        To = verificationRequest.Email,
                        Subject = $"Verification Code {code}",
                        HtmlBody = $@"
                         <!DOCTYPE html>
                         <html lang='en'>

                         <head>
                             <meta charset='UTF-8'>
                             <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                             <title>Verifieringskod</title>

                         </head>

                         <body style='font-family: Arial, sans-serif;
                         background-color: #f4f4f4;
                         margin: 0;
                         padding: 0;'>
                             <div style='width: 100%; max-width: 600px; margin: 20px auto; background-color: #fff; border-radius: 5px;'>
                                 <div style='text-align: center;padding: 20px; background-color: rgb(216, 230, 248);' >
                                     <div style='display: inline-block; height:120px; width:120px; background-color: #fff; border-radius: 50%;'>
                                         <p style='line-height: 120px; margin: 0; font-weight: bold; font-size: 22px;'>MANERO</p>
                                     </div>
                                 </div>
                                 <div style='text-align: center; padding: 20px; padding-top: 0; '>
                                     <p style='text-align: left; font-size: 18px; margin: 30px 0;'>Hello<br>
                                         You registered an account on Manero Fashion, before being able to use your account you need to verify
                                         your email address by entering the activation code in our app. <br>
                                     </p>
            
                                     <p style='text-align: center; font-size: 27px; background-color: rgb(216, 230, 248); padding:10px;'>{code}</p>
                                 </div>
                                 <div style='text-align: center; margin: 20px 0; color: #666;'>
                                     Kind Regards,<br>
                                     Manero Fashion
                                 </div>
                             </div>
                         </body>

                         </html>",
                        PlainText = $"Please verify your account using this verification code: {code}."
                    };

                    return emailRequest;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: GenerateVerificationCode.GenerateEmailRequest() :: {ex.Message}");

            }

            return null!;
        }

        public string GenerateServiceBusEmailRequest(EmailRequest emailRequest)
        {
            try
            {
                var payload = JsonConvert.SerializeObject(emailRequest);
                if (!string.IsNullOrEmpty(payload))
                    return payload;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: GenerateVerificationCode.GenerateServiceBusEmailRequest() :: {ex.Message}");

            }

            return null!;
        }
    }
}
