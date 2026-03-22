using PaymentAI.Core.DTOs;
using PaymentAI.Core.Entities;


namespace PaymentAI.Core.Interfaces.Services
{
    public interface IRiskRuleService
    {
        Task<IEnumerable<RuleResponseDto>> GetAllAsync();
        Task AddAsync(RuleRequestDto ruleDto);
        Task UpdateAsync(RuleRequestDto ruleDto);
        Task DeleteAsync(RuleRequestDto ruleDto);
        Task<int> EvaluateRiskAsync(Transaction transaction);
        bool EvaluateRule(RiskRule rule, Transaction transaction);
    }
}
