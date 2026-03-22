using Microsoft.EntityFrameworkCore;
using PaymentAI.Core.Entities;
using PaymentAI.Infrastructure.Data;


namespace PaymentAI.Infrastructure.Repositories
{
    public class RuleRepository : GenericRepository<RiskRule>, IRuleRepository
    {
        private readonly AppDbContext _context;

        public RuleRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RiskRule>> GetActiveRulesAsync()
        {
            return await _context.RiskRules
                    .Where(r => r.IsActive)
                    .AsNoTracking()
                    .ToListAsync();
        }
    }
}
