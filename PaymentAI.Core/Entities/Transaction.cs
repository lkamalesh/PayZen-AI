using PaymentAI.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAI.Core.Entities
{
    public class Transaction
    {
        [Key]
        public Guid TransactionId { get; set; }

        [Required]
        public string MerchantId { get; set; } = null!;

        [Required]
        public string CustomerId { get; set; } = null!;

        [Range(0.01, 1000000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3,MinimumLength = 3)]
        public string Currency { get; set; } = null!;

        [Required]
        public PaymentMethodType PaymentMethod { get; set; } 

        [Required]
        public TransactionStatus Status { get; set; } 

        [Required]
        [MaxLength(100)]
        public string? IdempotencyKey { get; set; }

        [Range(0, 100)]
        public int RiskScore { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public ApplicationUser? Merchant { get; set; }  
        public Customer? Customer { get; set; } 


    }
}
