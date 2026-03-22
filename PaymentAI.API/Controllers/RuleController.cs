using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentAI.Core.DTOs;
using PaymentAI.Core.Interfaces.Services;

namespace PaymentAI.API.Controllers
{
    [Authorize(Roles = "Analyst")]
    [Route("api/[controller]")]
    [ApiController]
    public class RuleController : ControllerBase
    {
        private readonly IRiskRuleService _ruleService;
        public RuleController(IRiskRuleService ruleservice)
        {
            _ruleService = ruleservice;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var rules = await _ruleService.GetAllAsync();
            return Ok(rules);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(RuleRequestDto ruleDto)
        {
            await _ruleService.AddAsync(ruleDto);
            return Created();
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update(RuleRequestDto ruleDto)
        {
            await _ruleService.UpdateAsync(ruleDto);
            return NoContent();
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete(RuleRequestDto ruleDto)
        {
            await _ruleService.DeleteAsync(ruleDto);
            return NoContent();
        }
    }
}
