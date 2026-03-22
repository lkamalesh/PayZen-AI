using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PaymentAI.Core.DTOs;
using PaymentAI.Core.Entities;
using PaymentAI.Infrastructure.Services;

namespace PaymentAI.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(UserManager<ApplicationUser> usermanager, AuthService authservice, ILogger<AuthController> logger) 
        {
            _userManager = usermanager;
            _authService = authservice;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterMerchantDto register)
        {
            _logger.LogInformation($"Register merchant request received for username {register.UserName} and email {register.Email}");

            var merchant = new ApplicationUser
            {
                Email = register.Email,
                UserName = register.UserName,
                FullName = register.UserName,
                Country = register.Country,
                ApiKey = Guid.NewGuid().ToString(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(merchant, register.Password);

            if (!result.Succeeded)
            {
                _logger.LogWarning($"Register merchant failed for email {register.Email}. Errors: {string.Join("; ", result.Errors.Select(e => e.Description))}");
                return BadRequest(result.Errors);
            }

            await _userManager.AddToRoleAsync(merchant, "Merchant");
            var merchantId = await _userManager.GetUserIdAsync(merchant);
            var merchantResponse = new MerchantResponseDto
            {
                MerchantId = merchantId,
                Name = merchant.UserName,
                Country = merchant.Country,
                ApiKey = merchant.ApiKey
            };

            _logger.LogInformation($"Register successful for MerchantId {merchantId}");

            return Ok(merchantResponse);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            _logger.LogInformation($"Login request received for email {request.Email}");

            var user = await _userManager.FindByEmailAsync(request.Email);
            
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                _logger.LogWarning($"Login failed for email {request.Email}");
                return Unauthorized(new { message = "Invalid email or password" });
            }
            Console.WriteLine(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            var token = _authService.GenerateJwtToken(user, userRoles);

            _logger.LogInformation($"Login successful for user {user.Id}");
            return Ok(new { Token = token });
        }
    }
}
