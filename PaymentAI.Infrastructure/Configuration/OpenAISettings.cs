using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentAI.Infrastructure.Configuration
{
    public class OpenAISettings
    {
        public required string Endpoint { get; set; }
        public required string ApiKey { get; set; }
        public required string AgentName { get; set; }
        public required string DeploymentName { get; set; }
    }
}
