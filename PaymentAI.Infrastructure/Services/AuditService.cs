using Microsoft.EntityFrameworkCore;
using PaymentAI.Core.DTOs;
using PaymentAI.Core.Entities;
using PaymentAI.Core.Interfaces.Services;
using PaymentAI.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAI.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly AppDbContext _context;
        public AuditService(AppDbContext context)
        {
            _context = context;
        }
        public Task LogAsync(string merchantId, string customerId, string action, DateTime timestamp)
        {
            var log = new AuditLog
            {
                LogId = Guid.NewGuid(),
                CustomerId = customerId,
                Action = action,
                Timestamp = timestamp
            };

            _context.AuditLogs.Add(log);
            return _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLogResponseDto>> GetAllLogsAsync()
        {
            return await _context.AuditLogs
                .Select(log => new AuditLogResponseDto
                {
                    CustomerId = log.CustomerId,
                    Action = log.Action,
                    Timestamp = log.Timestamp
                }).ToListAsync();
        }
    }
}
