using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VerificationProvider.Models;
using VerificationProvider.Services;

namespace VerificationProvider.Functions
{
    public class GenerateCodeHttp
    {
        private readonly ILogger<GenerateCodeHttp> _logger;
        private readonly IVerificationService _verificationService;

        public GenerateCodeHttp(ILogger<GenerateCodeHttp> logger, IVerificationService verificationService)
        {
            _logger = logger;
            _verificationService = verificationService;
        }

        [Function("GenerateCodeHttp")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "generate")] HttpRequest req)
        {
            try
            {
                VerificationRequest verificationRequest = new VerificationRequest();
                verificationRequest.Email = "cissi.aolsson@gmail.com";
                var result = await _verificationService.SaveVerificationRequest(verificationRequest, "123456");
                if (result)
                {
                    return new OkObjectResult(result);
                }

                return new StatusCodeResult(500);
                //var data = !string.IsNullOrEmpty(req.Query["email"])
                //    ? req.Query["email"].ToString()
                //    : await new StreamReader(req.Body).ReadToEndAsync();

                //if (string.IsNullOrEmpty(data))
                //    return new BadRequestObjectResult(new { Status = 400, Message = "Invalid incoming request" });


                //var incomingRequest = _verificationService.ConvertToIncomingRequest(data);
                //incomingRequest.Code = _verificationService.GeneratedCode();

                //if (await _verificationService.SaveIncomingRequestAsync(incomingRequest))
                //{
                //    var emailRequest = _verificationService.GenerateEmailRequest(incomingRequest);
                //    var serviceBusMessage = _verificationService.GenerateServiceBusMessage(JsonConvert.SerializeObject(emailRequest),


                //     _verificationService.SendServiceBusMessageAsync(serviceBusMessage);



                //}
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: GenerateAndSendVerificationCode.Run() :: {ex.Message}");

            }
            return null!;
        }
    }
}
