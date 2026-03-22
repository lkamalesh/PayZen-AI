using AutoMapper;
using PaymentAI.Core.DTOs;
using PaymentAI.Core.Entities;


namespace PaymentAI.Infrastructure.Mappings
{
    public class RiskRuleProfile : Profile
    {
        public RiskRuleProfile()
        {
            CreateMap<RuleRequestDto, RiskRule>();
            CreateMap<RiskRule, RuleResponseDto>();
        }
    }
}
