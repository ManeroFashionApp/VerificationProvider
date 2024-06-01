
namespace VerificationProvider.Infrastructure.Services
{
    public interface IVerificationCleanerService
    {
        Task RemoveExpiredRecordsAsync();
    }
}