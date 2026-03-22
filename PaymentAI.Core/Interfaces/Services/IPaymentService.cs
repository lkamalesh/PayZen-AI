using PaymentAI.Core.DTOs;


namespace PaymentAI.Core.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> ProcessPaymentAsync(PaymentRequestDto request, string idempotencyKey);
    }
}
