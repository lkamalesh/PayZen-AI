using PaymentAI.Core.Entities;

namespace PaymentAI.Core.Interfaces.Services
{
    public interface IRiskExplanationService
    {
        Task<string> GenerateExplanationAsync(Transaction txn);
    }
}
        