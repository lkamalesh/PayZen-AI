using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentAI.Core.Entities;
using PaymentAI.Core.Interfaces.Services;

namespace PaymentAI.API.Controllers
{
    [Authorize(Roles = "Analyst")]
    [Route("api/[controller]")]
    [ApiController]
    public class MerchantController : ControllerBase
    {
        private readonly IMerchantService _merchantService;

        public MerchantController(IMerchantService merchantService)
        {
            _merchantService = merchantService;
        }

        [HttpGet("{merchantId}")]
        public async Task<IActionResult> GetMerchantByIdAsync(string merchantId)
        {
            var merchant = await _merchantService.GetMerchantByIdAsync(merchantId);
            if (merchant == null)
            {
                return NotFound();
            }
            return Ok(merchant);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllMerchantsAsync()
        {
            var merchants = await _merchantService.GetAllMerchantsAsync();
            return Ok(merchants);
        }

    }
}
