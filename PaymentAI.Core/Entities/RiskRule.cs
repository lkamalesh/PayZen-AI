using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAI.Core.Entities
{
    public class RiskRule
    {
        [Key]
        public Guid RuleId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Condition { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Value { get; set; } = null!;

        public int ScoreImpact { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
