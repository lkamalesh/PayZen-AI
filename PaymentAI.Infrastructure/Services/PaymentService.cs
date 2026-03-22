using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PaymentAI.Core.DTOs;
using PaymentAI.Core.Entities;
using PaymentAI.Core.Enums;
using PaymentAI.Core.Interfaces.Repository;
using PaymentAI.Core.Interfaces.Services;


namespace PaymentAI.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ITransactionRepository _transactRepo;
        private readonly IRiskRuleService _riskService;
        private readonly ICustomerRepository _customerRepo;
        private readonly IAuditService _auditService;
        private readonly IMerchantService _merchantService;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(ITransactionRepository transact, 
                              IRiskRuleService riskService, 
                              ICustomerRepository customerRepo, 
                              IAuditService auditService,
                              IMerchantService merchantService,
                              ILogger<PaymentService> logger)
        {
            _transactRepo = transact;
            _riskService = riskService;
            _customerRepo = customerRepo;
            _auditService = auditService;
            _merchantService = merchantService; 
            _logger = logger;
        }

        public async Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto request, string idempotencyKey)
        {

            // Check if Merchant exists and is active
            var merchant = await _merchantService.GetMerchantByIdAsync(request.MerchantId); 

            if (merchant == null || !merchant.IsActive)
            {
                _logger.LogWarning($"Payment rejected due to invalid or inactive merchant {merchant.MerchantId}");
                await _auditService.LogAsync(merchant.MerchantId, request.CustomerId, "Payment_InvalidMerchant", DateTime.UtcNow);
                return new PaymentResponseDto
                {
                    Message = "Invalid Merchant!"
                };
            }

            // Idempotency: To avoid Duplicate Transactions due to retries.
            var existingTransact = await _transactRepo.GetByIdempotencyKeyAsync(idempotencyKey);

            if (existingTransact != null)
            {
                await _auditService.LogAsync(merchant.MerchantId, existingTransact.CustomerId, "Payment_IdempotencyHit", DateTime.UtcNow);
                return new PaymentResponseDto
                {
                    Message = "Transaction already processed"
                };
            }

            // Check if Customer exists, if not create a new one. This allows to track customer behavior over time.
            var customer = await _customerRepo.GetCustomerByIdAsync(request.CustomerId, merchant.MerchantId);

            if (customer == null)
            {
                customer = new Customer
                {
                    CustomerId = request.CustomerId,
                    MerchantId = request.MerchantId,
                    Country = request.Country, 
                    CreatedAt = DateTime.UtcNow
                };
                await _customerRepo.AddAsync(customer);
                await _customerRepo.SaveAsync();
                _logger.LogInformation($"Created new customer {customer.CustomerId} for merchant {customer.MerchantId}");
            }
            else
            {
                customer.Country = request.Country;
                _logger.LogDebug($"Updated customer {customer.CustomerId} country to {customer.Country}");
            }

            // Create a new Transaction with status Pending. 
            var transaction = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                MerchantId = request.MerchantId,
                CustomerId = request.CustomerId,
                Amount = request.Amount,
                Currency = request.Currency,
                PaymentMethod = request.PaymentMethod,
                IdempotencyKey = idempotencyKey,
                Status = TransactionStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Customer = customer // set customer instance
            };

            await _transactRepo.AddAsync(transaction);
            await _transactRepo.SaveAsync();
            
            // Evaluate Risk using the RiskRuleService. 
            var riskScore = await _riskService.EvaluateRiskAsync(transaction);

            transaction.RiskScore = riskScore;
            if (riskScore < 40)
                transaction.Status = TransactionStatus.Authorized;

            else if (riskScore < 70)
                transaction.Status = TransactionStatus.Flagged;   

            else
                transaction.Status = TransactionStatus.Declined;

            _transactRepo.Update(transaction);
            await _transactRepo.SaveAsync();

            await _auditService.LogAsync(merchant.MerchantId, customer.CustomerId, $"Payment_Processed_{transaction.Status}", DateTime.UtcNow);

            return new PaymentResponseDto
            {
                TransactionId = transaction.TransactionId,
                Status = transaction.Status,
                RiskScore = transaction.RiskScore,
                ProcessedAt = DateTime.UtcNow,
                Message = "Transaction processed successfully"
            };


        }

    }
}
