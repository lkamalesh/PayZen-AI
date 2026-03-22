using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAI.Core.DTOs
{
    public class RuleRequestDto
    {
        [Required]
        public Guid RuleId { get; set; } 

        [Required]
        [MaxLength(100)]
        public required string Condition { get; set; } 

        [Required]
        [MaxLength(50)]
        public required string Value { get; set; } 

        public int ScoreImpact { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
