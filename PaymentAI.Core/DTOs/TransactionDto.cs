using PaymentAI.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAI.Core.DTOs
{
     public class TransactionDto
     {
        public Guid TransactionId { get; set; }
        public Guid MerchantId { get; set; } 
        public Guid CustomerId { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public required string Currency { get; set; } 

        [Required]
        public PaymentMethodType PaymentMethod { get; set; } 

        [Required]
        [MaxLength(30)]
        public required string Status { get; set; }

        [Range(0, 100)]
        public int RiskScore { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
