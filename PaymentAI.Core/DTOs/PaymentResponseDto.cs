using PaymentAI.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAI.Core.DTOs
{
    public class PaymentResponseDto
    {
        public Guid TransactionId { get; set; }
        public TransactionStatus Status { get; set; } 
        public int RiskScore { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }
    }
}
