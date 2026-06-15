using Microsoft.AspNetCore.Mvc;
using MovieManager.API.DTOs.UserDTOs; // Ensure this matches your DTO namespace
using MovieManager.API.Services;

namespace MovieManager.API.Controllers
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
        private readonly iUserService _userService;
        private readonly ILogger<UsersController> _logger;

        /// <summary>
        /// Initializes a new instance of the UsersController class.
        /// </summary>
        /// <param name="userService">The user service for handling business logic.</param>
        /// <param name="logger">Logger for recording controller actions.</param>
        public UsersController(iUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all users from the database.
        /// </summary>
        /// <returns>A collection of all users.</returns>
        /// <response code="200">Returns the list of all users.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
        {
            _logger.LogInformation("GET request: Retrieve all users");

            var users = await _userService.GetAll();
            return Ok(users);
        }

        /// <summary>
        /// Gets a specific user by their ID.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>The user with the specified ID.</returns>
        /// <response code="200">Returns the requested user.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> GetUserById(int id)
        {
            _logger.LogInformation("GET request: Retrieve user with ID {userId}", id);

            var user = await _userService.GetById(id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return Ok(user);
        }

        /// <summary>
        /// Gets a specific user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>The user with the specified email.</returns>
        /// <response code="200">Returns the requested user.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpGet("search/email/{email}")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> GetUserByEmail(string email)
        {
            _logger.LogInformation("GET request: Search user by email '{email}'", email);

            var user = await _userService.GetByEmail(email);

            if (user == null)
            {
                return NotFound($"User with email '{email}' not found.");
            }

            return Ok(user);
        }

        /// <summary>
        /// Creates a new user in the database.
        /// </summary>
        /// <param name="createUserDto">The user data to create.</param>
        /// <returns>The created user.</returns>
        /// <response code="201">User successfully created.</response>
        /// <response code="400">Invalid user data provided or email/username already exists.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> CreateUser([FromBody] CreateUserDTO createUserDto)
        {
            _logger.LogInformation("POST request: Create new user");

            try
            {
                // Passing the DTO directly to the service
                var createdUser = await _userService.CreateUser(createUserDto);

                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Updates an existing user in the database.
        /// </summary>
        /// <param name="id">The unique identifier of the user to update.</param>
        /// <param name="updateUserDto">The updated user data.</param>
        /// <returns>No content response on successful update.</returns>
        /// <response code="204">User successfully updated.</response>
        /// <response code="400">Invalid user data provided.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO updateUserDto)
        {
            _logger.LogInformation("PUT request: Update user with ID {userId}", id);

            try
            {
                // Passing the DTO directly to the service
                await _userService.UpdateUser(id, updateUserDto);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Deletes a user from the database.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <returns>No content response on successful deletion.</returns>
        /// <response code="204">User successfully deleted.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation("DELETE request: Delete user with ID {userId}", id);

            try
            {
                await _userService.DeleteUser(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Authenticates a user based on email and password.
        /// </summary>
        /// <param name="loginDto">The login credentials.</param>
        /// <returns>The authenticated user's DTO.</returns>
        /// <response code="200">Successfully authenticated.</response>
        /// <response code="401">Invalid email or password.</response>
        /// <response code="500">Internal server error occurred.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserDTO>> Login([FromBody] LoginDTO loginDto)
        {
            _logger.LogInformation("POST request: Login attempt for email {email}", loginDto.UserName);

            var user = await _userService.Login(loginDto);

            if (user == null)
            {
                // 401 Unauthorized is the standard response for failed logins
                return Unauthorized("Invalid username or password.");
            }

            return Ok(user);
        }
    }
}