using PaymentAI.Core.DTOs;
using PaymentAI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAI.Core.Interfaces.Services
{
    public interface IMerchantService
    {
        Task<MerchantResponseDto?> GetMerchantByIdAsync(string merchantId);
        Task<IEnumerable<MerchantResponseDto>> GetAllMerchantsAsync();
        Task<MerchantResponseDto?> GetMerchantByApikeyAsync(string apiKey);
    }
}
