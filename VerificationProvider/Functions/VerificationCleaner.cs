using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VerificationProvider.Functions;

public class VerificationCleaner
{
    private readonly ILogger<VerificationCleanerService> _logger;
    private readonly IVerificationCleanerService _verificationCleanerService;

    public VerificationCleaner(ILogger<VerificationCleanerService> logger, IVerificationCleanerService verificationCleanerService)
    {
        _logger = logger;
        _verificationCleanerService = verificationCleanerService;
    }

    [Function("VerificationCleaner")]
    public async Task Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer)
    {
        try
        {
            await _verificationCleanerService.RemoveExpiredRecordsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: VerificationCleaner.Run() :: {ex.Message}");

        }
    }
}
