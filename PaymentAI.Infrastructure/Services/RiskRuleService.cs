using AutoMapper;
using PaymentAI.Core.DTOs;
using PaymentAI.Core.Entities;
using PaymentAI.Core.Interfaces.Repository;
using PaymentAI.Core.Interfaces.Services;
using PaymentAI.Infrastructure.Repositories;


namespace PaymentAI.Infrastructure.Services
{
    public class RiskRuleService : IRiskRuleService
    {
        private readonly IRuleRepository _ruleRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly IMapper _mapper;

        public RiskRuleService(IRuleRepository riskRule, ITransactionRepository transactionRepo, IMapper mapper)
        {
            _ruleRepo = riskRule;
            _transactionRepo = transactionRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RuleResponseDto>> GetAllAsync()
        {
            var rules = await _ruleRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<RuleResponseDto>>(rules);
        }

        public async Task AddAsync(RuleRequestDto ruleDto)
        {
            var rule = _mapper.Map<RiskRule>(ruleDto);

            await _ruleRepo.AddAsync(rule);
            await _ruleRepo.SaveAsync();
        }

        public async Task UpdateAsync(RuleRequestDto ruleDto)
        {
            var rule = await _ruleRepo.GetByIdAsync(ruleDto.RuleId);
            if (rule != null)
            {
                rule.Condition = ruleDto.Condition;
                rule.Value = ruleDto.Value;
                rule.ScoreImpact = ruleDto.ScoreImpact;
                rule.Description = ruleDto.Description;
                rule.IsActive = ruleDto.IsActive;

                _ruleRepo.Update(rule);
                await _ruleRepo.SaveAsync();
            }
        }

        public async Task DeleteAsync(RuleRequestDto ruleDto)
        {
            var rule = await _ruleRepo.GetByIdAsync(ruleDto.RuleId);
            if (rule != null)
            {
                _ruleRepo.Delete(rule);
                await _ruleRepo.SaveAsync();
            }
        }
        public async Task<int> EvaluateRiskAsync(Transaction transaction)
        {
            int riskScore = 0;

            var rules = await _ruleRepo.GetActiveRulesAsync();

            foreach (var rule in rules)
            {
                if (EvaluateRule(rule, transaction))
                {
                    riskScore += rule.ScoreImpact;
                }
            }

            // Velocity checks in multiple time windows.
            var lastMinuteTransactions = await _transactionRepo.GetTransactionCountSinceAsync(transaction.CustomerId, DateTime.UtcNow.AddMinutes(-1));
            var lastHourTransactions = await _transactionRepo.GetTransactionCountSinceAsync(transaction.CustomerId, DateTime.UtcNow.AddHours(-1));

            if (lastMinuteTransactions >= 5)  riskScore += 10;

            if (lastHourTransactions >= 20)  riskScore += 20;

            return Math.Clamp(riskScore, 0, 100);
        }

        public bool EvaluateRule(RiskRule rule, Transaction transaction)
        {
            switch (rule.Condition)
            { 
                case "AmountGreaterThan":
                    if (decimal.TryParse(rule.Value, out var amount))
                        return transaction.Amount > amount;
                    break;

                case "CurrencyIs":
                    return string.Equals(transaction.Currency, rule.Value, StringComparison.OrdinalIgnoreCase);

                case "PaymentMethodIs":
                    return string.Equals(transaction.PaymentMethod.ToString(), rule.Value, StringComparison.OrdinalIgnoreCase);

                case "CustomerCountryIs":
                    return transaction.Customer != null &&
                           string.Equals(transaction.Customer.Country, rule.Value, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
    }
}
