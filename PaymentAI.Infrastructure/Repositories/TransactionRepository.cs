using Microsoft.EntityFrameworkCore;
using PaymentAI.Core.Entities;
using PaymentAI.Core.Enums;
using PaymentAI.Core.Interfaces.Repository;
using PaymentAI.Infrastructure.Data;
using System.Xml.Schema;


namespace PaymentAI.Infrastructure.Repositories
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Transaction>> GetMerchantTransactionsAsync(string merchantId)
        {
            return await _context.Transactions
                .AsNoTracking()
                .Where(t => t.MerchantId == merchantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetRecentTransactionsAsync(string customerId, int limit)
        {
            return await _context.Transactions
                .AsNoTracking()
                .Where(t => t.CustomerId == customerId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Transaction?> GetByIdempotencyKeyAsync(string idempotencyKey)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.IdempotencyKey == idempotencyKey);
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsByStatusAsync(TransactionStatus status)
        { 
            return await _context.Transactions
                .AsNoTracking()
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
        public async Task<IEnumerable<Transaction>> GetFlaggedTransactionsAsync()
        {
            return await _context.Transactions
                .AsNoTracking()
                .Where(t => t.Status == TransactionStatus.Declined)
                .OrderByDescending(t => t.CreatedAt)
                .Take(50)
                .ToListAsync();
        }

        public async Task<int> GetTransactionCountSinceAsync(string customerId, DateTime sinceUtc)
        {
            return await _context.Transactions
                .Where(t => t.CustomerId == customerId && t.CreatedAt >= sinceUtc)
                .CountAsync();
        }

    }
}
