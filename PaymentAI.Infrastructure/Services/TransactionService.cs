using PaymentAI.Core.Entities;
using PaymentAI.Core.Enums;
using PaymentAI.Core.Interfaces.Repository;
using PaymentAI.Core.Interfaces.Services;

namespace PaymentAI.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepo;
        public TransactionService(ITransactionRepository transactRepo)
        {
         _transactionRepo = transactRepo;
        }

        public async Task<Transaction?> GetByIdAsync(Guid transactionId)
        {
            return await _transactionRepo.GetByIdAsync(transactionId);
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
        {
            return await _transactionRepo.GetAllAsync();
        }
        public async Task<IEnumerable<Transaction>> GetMerchantTransactionsAsync(string merchantId)
        {
            return await _transactionRepo.GetMerchantTransactionsAsync(merchantId);
        }
        public async Task<IEnumerable<Transaction>> GetRecentTransactionsAsync(string customerId, int limit)
        {
            if (limit <= 0 || limit > 50)
            {
                limit = 10;
            }

            return await _transactionRepo.GetRecentTransactionsAsync(customerId, limit);
        }

        public async Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey)
        {
            return await _transactionRepo.GetByIdempotencyKeyAsync(idempotencyKey);
        }
        public async Task<IEnumerable<Transaction>> GetTransactionsByStatusAsync(TransactionStatus status)
        {
            return await _transactionRepo.GetTransactionsByStatusAsync(status);
        }
       
    }
}
