using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAI.Core.Entities
{
    public class AuditLog
    {
        [Key]
        public Guid LogId { get; set; }

        [Required]
        public string CustomerId { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Action { get; set; } = null!;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string? IPAddress { get; set; }

        public Customer? Customer { get; set; }
    }
}
