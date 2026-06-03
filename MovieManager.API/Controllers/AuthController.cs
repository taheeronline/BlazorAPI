using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MovieManager.API.Services;
using MovieManager.API.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace MovieManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;
        private readonly IUserService _userService;
        private readonly MovieDbContext _dbContext;

        public AuthController(IConfiguration configuration, ILogger<AuthController> logger, IUserService userService, MovieDbContext dbContext)
        {
            _configuration = configuration;
            _logger = logger;
            _userService = userService;
            _dbContext = dbContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var existingUser = await _userService.FindByUsernameAsync(request.Username);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Username already exists" });
                }

                var user = await _userService.CreateUserAsync(request.Username, request.Email, request.Password, "User");
                return CreatedAtAction(nameof(Register), new { userId = user.Id }, new { message = "User created successfully", username = user.Username, email = user.Email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return StatusCode(500, new { message = "An error occurred while creating the user" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.FindByUsernameAsync(request.Username);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var valid = await _userService.ValidatePasswordAsync(user, request.Password);
            if (!valid)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Record login
            await _userService.RecordLoginAsync(user);

            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection.GetValue<string>("Key") ?? "ChangeThisToAStrongSecretInProduction12345!";
            var issuer = jwtSection.GetValue<string>("Issuer") ?? "MovieManagerAPI";
            var audience = jwtSection.GetValue<string>("Audience") ?? "MovieManagerClients";
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(key);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(jwtSection.GetValue<int>("TokenLifetimeMinutes", 60)),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString, User = new { Username = user.Username, Role = user.Role, Email = user.Email } });
        }

        [HttpGet("loginlogs")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetLoginLogs()
        {
            try
            {
                var logs = await _dbContext.LoginLogs
                    .Include(ll => ll.User)
                    .Select(ll => new
                    {
                        ll.Id,
                        ll.UserId,
                        Username = ll.User!.Username,
                        ll.LoginTime,
                        ll.LogoutTime,
                        ll.IpAddress,
                        ll.UserAgent
                    })
                    .ToListAsync();

                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching login logs");
                return StatusCode(500, new { message = "An error occurred while fetching login logs" });
            }
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
