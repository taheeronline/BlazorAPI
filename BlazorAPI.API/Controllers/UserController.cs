using Microsoft.AspNetCore.Mvc;
using BlazorAPI.API.DTOs.UserDTOs;
using BlazorAPI.API.Exceptions;
using BlazorAPI.API.Services.Interface;
using BlazorAPI.API.Wrapper;

namespace BlazorAPI.API.Controllers
{
    /// <summary>
    /// API controller for managing users.
    /// Provides endpoints for CRUD operations and authentication.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        /// <summary>
        /// Initializes a new instance of the UsersController class.
        /// </summary>
        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets a paginated list of all users.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<UserDTO>>> GetAllUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET request: Retrieve users page {Page}", page);
            var users = await _userService.GetAllAsync(page, pageSize, cancellationToken);
            return Ok(users);
        }

        /// <summary>
        /// Gets a specific user by their ID.
        /// </summary>
        // CHANGE: Added :int route constraint
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> GetUserById(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET request: Retrieve user with ID {UserId}", id);

            try
            {
                var user = await _userService.GetByIdAsync(id, cancellationToken);
                if (user == null)
                {
                    return NotFound(new { message = $"User with ID {id} not found." });
                }
                return Ok(user);
            }
            catch (UserValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Gets a specific user by their email address.
        /// </summary>
        [HttpGet("search/email/{email}")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> GetUserByEmail([FromRoute] string email, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET request: Search user by email '{Email}'", email);

            try
            {
                var user = await _userService.GetByEmailAsync(email, cancellationToken);
                if (user == null)
                {
                    return NotFound(new { message = $"User with email '{email}' not found." });
                }
                return Ok(user);
            }
            catch (UserValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new user in the database.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> CreateUser([FromBody] CreateUserDTO createUserDto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST request: Create new user");

            try
            {
                var createdUser = await _userService.CreateUserAsync(createUserDto, cancellationToken);
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }
            catch (UserValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing user in the database.
        /// </summary>
        // CHANGE: Added :int route constraint
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> UpdateUser(int id, [FromBody] UpdateUserDTO updateUserDto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PUT request: Update user with ID {UserId}", id);

            try
            {
                var updatedUser = await _userService.UpdateUserAsync(id, updateUserDto, cancellationToken);
                return Ok(updatedUser);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UserValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a user from the database.
        /// </summary>
        // CHANGE: Added :int route constraint
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(int id, [FromQuery] int modifiedById, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("DELETE request: Delete user with ID {UserId}", id);

            try
            {
                await _userService.DeleteUserAsync(id, modifiedById, cancellationToken);
                return NoContent();
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UserValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Authenticates a user based on email and password.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> Login([FromBody] LoginDTO loginDto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST request: Login attempt for username {UserName}", loginDto.UserName);

            try
            {
                var user = await _userService.LoginAsync(loginDto, cancellationToken);

                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid username or password." });
                }

                return Ok(user);
            }
            catch (UserValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}