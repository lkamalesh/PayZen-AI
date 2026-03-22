using PaymentAI.Core.Entities;
using PaymentAI.Core.Interfaces.Repository;


namespace PaymentAI.Infrastructure.Repositories
{
    public interface IRuleRepository : IGenericRepository<RiskRule>
    {
        Task<IEnumerable<RiskRule>> GetActiveRulesAsync();
    }
}
