using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VerificationProvider.Data.Contexts;
using VerificationProvider.Infrastructure.Models;



namespace VerificationProvider.Infrastructure.Services;

public class ValidateCodeService : IValidateCodeService
{
    private readonly ILogger<ValidateCodeService> _logger;
    private readonly DataContext _context;

    public ValidateCodeService(ILogger<ValidateCodeService> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<ValidateCodeRequest> UnpackValidateRequestAsync(HttpRequest req)
    {
        try
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync(); 
            if (!string.IsNullOrEmpty(body))
            {
                var validateRequest = JsonConvert.DeserializeObject<ValidateCodeRequest>(body);
                if (validateRequest != null)
                    return validateRequest;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: ValidateVerificationCode.UnpackValidateRequestAsync() :: {ex.Message}");

        }
        return null!;
    }

    public async Task<bool> ValidateCodeAsync(ValidateCodeRequest validateRequest)
    {
        try
        {
            var entity = await _context.IncomingRequests
                .FirstOrDefaultAsync(x => x.Email == validateRequest.Email && x.Code == validateRequest.Code);

            if (entity != null)
            {
                _context.IncomingRequests.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: ValidateVerificationCode.ValidateCodeAsync() :: {ex.Message}");

        }

        return false;
    }
}
