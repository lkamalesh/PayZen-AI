using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAI.Core.DTOs
{
    public class MerchantResponseDto
    {
        public string MerchantId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string ApiKey { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
