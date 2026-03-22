using PaymentAI.Core.Entities;
using PaymentAI.Core.Enums;


namespace PaymentAI.Core.Interfaces.Repository
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetRecentTransactionsAsync(string customerId, int limit);
        Task<IEnumerable<Transaction>> GetFlaggedTransactionsAsync();
        Task<IEnumerable<Transaction>> GetMerchantTransactionsAsync(string merchantId);

        Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey);
        Task<IEnumerable<Transaction>> GetTransactionsByStatusAsync(TransactionStatus status);
        Task<int> GetTransactionCountSinceAsync(string customerId, DateTime sinceUtc);
    }
}
