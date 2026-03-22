#pragma warning disable OPENAI001
#pragma warning disable AAIP001
using Azure.AI.Extensions.OpenAI;
using OpenAI.Responses;
using PaymentAI.Core.Entities;
using PaymentAI.Core.Interfaces.Services;



namespace PaymentAI.Infrastructure.Services
{
    public class RiskExplanationService : IRiskExplanationService
    {
        private readonly ProjectResponsesClient _responseClient;

        public RiskExplanationService(ProjectResponsesClient responseClient)
        {
            _responseClient = responseClient;
        }

        public async Task<string> GenerateExplanationAsync(Transaction txn)
        {
            var prompt = $@"
                You are an AML compliance expert.

                Use ONLY the provided files to explain the risk.

                 Transaction:
                - Amount: {txn.Amount}
                - Currency: {txn.Currency}
                - Country: {txn.Customer?.Country}

                Task:
                Explain clearly why this transaction is considered high risk.

                Include:
                - Relevant AML rules or thresholds
                - Country or jurisdiction risks
                - Suspicious transaction indicators
                - Add Source files name at the end

                Keep the explanation concise and professional.
                ";

            ResponseResult response = await _responseClient.CreateResponseAsync(prompt);

            return response.GetOutputText();
        }
    }
}
