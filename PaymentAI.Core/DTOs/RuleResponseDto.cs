using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAI.Core.DTOs
{
    public class RuleResponseDto
    {
        public Guid RuleId { get; set; }

        public string Condition { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public int ScoreImpact { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
