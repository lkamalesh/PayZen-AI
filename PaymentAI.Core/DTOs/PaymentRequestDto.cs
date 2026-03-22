using PaymentAI.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAI.Core.DTOs
{
    public class PaymentRequestDto
    {
        [Required]
        public required string MerchantId { get; set; }
        [Required]
        public required string CustomerId { get; set; }

        [Range(0.01, 1000000)]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Country { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        public required string Currency { get; set; }

        [Required]
        [EnumDataType(typeof(PaymentMethodType))]
        public PaymentMethodType PaymentMethod { get; set; } 
    }
}
