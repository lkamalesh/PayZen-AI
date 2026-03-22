using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentAI.Core.Interfaces.Services;

namespace PaymentAI.API.Controllers
{
    [Authorize(Roles = "Analyst")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllLogsAsync()
        {
            var logs = await _auditService.GetAllLogsAsync();
            return Ok(logs);
        }
    }
}
