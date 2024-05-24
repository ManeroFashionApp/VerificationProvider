using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace VerificationProvider.Services;

public class VerificationCleanerService : IVerificationCleanerService
{
    private readonly ILogger<VerificationCleanerService> _logger;
    private readonly DataContext _context;

    public VerificationCleanerService(ILogger<VerificationCleanerService> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task RemoveExpiredRecordsAsync()
    {
        try
        {
            var expired = await _context.IncomingRequests.Where(x => x.ExpiryDate <= DateTime.Now).ToListAsync();
            _context.RemoveRange(expired);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: VerificationCleanerService.RemoveExpiredRecordsAsync() :: {ex.Message}");

        }
    }
}
