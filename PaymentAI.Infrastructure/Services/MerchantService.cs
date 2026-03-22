using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PaymentAI.Core.DTOs;
using PaymentAI.Core.Entities;
using PaymentAI.Core.Interfaces.Services;


namespace PaymentAI.Infrastructure.Services
{
    public class MerchantService : IMerchantService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public MerchantService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<MerchantResponseDto?> GetMerchantByIdAsync(string merchantId)
        {
            var merchant = await _userManager.FindByIdAsync(merchantId);
            if (merchant == null)
            {
                return null;
            }
            return new MerchantResponseDto
            {
                MerchantId = merchant.Id,
                Name = merchant.FullName,
                Email = merchant.Email ?? string.Empty,
                Country = merchant.Country,
                ApiKey = merchant.ApiKey,
                IsActive = merchant.IsActive
            };
        }
        public async Task<IEnumerable<MerchantResponseDto>> GetAllMerchantsAsync()
        {
            var merchants = await _userManager.Users.ToListAsync();

            return merchants.Where(m => m.FullName != "System Analyst" && m.FullName != "System Admin")
                .Select(m => new MerchantResponseDto
                    {
                        MerchantId = m.Id,
                        Name = m.FullName,
                        Email = m.Email ?? string.Empty,
                        Country= m.Country,
                        ApiKey = m.ApiKey,
                    IsActive = m.IsActive,
                    });
        }

        public async Task<MerchantResponseDto?> GetMerchantByApikeyAsync(string apiKey)
        {
            var merchant = await _userManager.Users.FirstOrDefaultAsync(m => m.ApiKey == apiKey);

            if (merchant == null)
            {
                return null;
            }

            return new MerchantResponseDto
            {
                MerchantId = merchant.Id,
                Name = merchant.FullName,
                Email = merchant.Email ?? string.Empty,
                Country = merchant.Country,
                ApiKey = merchant.ApiKey,
                IsActive = merchant.IsActive
            };
        }
    }
}       
