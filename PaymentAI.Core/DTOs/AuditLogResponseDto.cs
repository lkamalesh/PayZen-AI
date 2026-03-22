using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAI.Core.DTOs
{
    public class AuditLogResponseDto
    {
        public string CustomerId { get; set; } = string.Empty;

        public string Action { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } 

        public string? IPAddress { get; set; }

    }
}
