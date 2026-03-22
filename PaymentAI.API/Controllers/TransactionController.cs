using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentAI.Core.Enums;
using PaymentAI.Core.Interfaces.Services;
using PaymentAI.Infrastructure.Services;
using System.Runtime.InteropServices;

namespace PaymentAI.API.Controllers
{
    [Authorize(Roles = "Analyst")]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllTransactionsAsync()
        {
            var transactions = await _transactionService.GetAllTransactionsAsync();
            return Ok(transactions);
        }

        [HttpGet("{merchantId}")]
        public async Task<IActionResult> GetMerchantTransactionsAsync(string merchantId)
        {   
            var transactions = await _transactionService.GetMerchantTransactionsAsync(merchantId);
 
            return Ok(transactions);
        }

        [HttpGet("GetRecentTransactions/{customerId}/{limit}")]
        public async Task<IActionResult> GetRecentTransactions(string customerId, int limit = 10)
        {
            var transactions = await _transactionService.GetRecentTransactionsAsync(customerId, limit);

            return Ok(transactions);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetTransactionsByStatusAsync(TransactionStatus status)
        {
            var transactions = await _transactionService.GetTransactionsByStatusAsync(status);
            return Ok(transactions);
        }
    }
}
