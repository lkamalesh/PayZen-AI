using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentAI.Core.Interfaces.Services;

namespace PaymentAI.API.Controllers
{
    [Authorize(Roles = "Analyst")]
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IRiskExplanationService _riskExplanationService;
        private readonly ITransactionService _transactionService;

        public AIController(IRiskExplanationService riskExplanationService, ITransactionService transactionService)
        {
            _riskExplanationService = riskExplanationService;
            _transactionService = transactionService;
        }

        [HttpGet("{transactionId}")]
        public async Task<IActionResult> GetExplanation(Guid transactionId)
        {
            var txn = await _transactionService.GetByIdAsync(transactionId);

            if (txn == null)
            {
                return NotFound($"Transaction with ID {transactionId} not found.");
            }
            var explanation = await _riskExplanationService.GenerateExplanationAsync(txn);
            return Ok(explanation);
        }
    }
}
