using PaymentAI.Core.Entities;
using PaymentAI.Core.Enums;


namespace PaymentAI.Core.Interfaces.Services
{
    public interface ITransactionService
    {
        Task<Transaction?> GetByIdAsync(Guid transactionId);
        Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task<IEnumerable<Transaction>> GetRecentTransactionsAsync(string customerId, int limit);

        Task<IEnumerable<Transaction>> GetMerchantTransactionsAsync(string merchantId);

        Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey);
        Task<IEnumerable<Transaction>> GetTransactionsByStatusAsync(TransactionStatus status);


    }
}
