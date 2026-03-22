using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentAI.Core.DTOs;
using PaymentAI.Core.Interfaces.Services;

namespace PaymentAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(
            [FromBody]PaymentRequestDto paymentRequest,
            [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
        {

            if (string.IsNullOrWhiteSpace(idempotencyKey))
            {
                _logger.LogWarning("Payment request rejected because Idempotency-Key header was missing");
                return BadRequest("Idempotency-Key header is missing.");
            }

            var request = await _paymentService.ProcessPaymentAsync(paymentRequest, idempotencyKey);
            _logger.LogInformation($"Payment request completed with status {request.Status} and transaction {request.TransactionId}");

            return Ok(request);
        }
    }
}
