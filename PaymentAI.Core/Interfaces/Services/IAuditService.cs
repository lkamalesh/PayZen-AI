using PaymentAI.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAI.Core.Interfaces.Services
{
    public interface IAuditService
    {
        Task LogAsync(string merchantId, string customerId, string action, DateTime timestamp);

        Task<IEnumerable<AuditLogResponseDto>> GetAllLogsAsync();
    }
}
