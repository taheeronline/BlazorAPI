using Microsoft.AspNetCore.Mvc;
using BlazorAPI.API.DTOs.BookDTO;
using BlazorAPI.API.Exceptions;
using BlazorAPI.API.Services.Interface;
using BlazorAPI.API.Wrapper;

namespace BlazorAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<BookDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<BookDTO>>> GetAllBooks(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET request: Retrieve books page {Page}", page);
            var books = await _bookService.GetAllAsync(page, pageSize, cancellationToken);
            return Ok(books);
        }

        // CHANGE: Added :int route constraint
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(BookDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookDTO>> GetBookById(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET request: Retrieve book by ID: {BookId}", id);
            try
            {
                var book = await _bookService.GetByIdAsync(id, cancellationToken);
                return Ok(book);
            }
            catch (BookValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (BookNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(BookDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookDTO>> CreateBook([FromBody] CreateBookDTO bookDto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST request: Create a new book");
            try
            {
                var createdBook = await _bookService.CreateBookAsync(bookDto, cancellationToken);
                return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, createdBook);
            }
            catch (BookValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // CHANGE: Added :int route constraint
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(BookDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BookDTO>> UpdateBook(int id, [FromBody] UpdateBookDTO bookDto, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PUT request: Update book with ID: {BookId}", id);
            try
            {
                var updatedBook = await _bookService.UpdateBookAsync(id, bookDto, cancellationToken);
                return Ok(updatedBook);
            }
            catch (BookNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BookValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // CHANGE: Added :int route constraint
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // CHANGE: Documented 400 response
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> DeleteBook(int id, [FromQuery] int modifiedById, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("DELETE request: Delete book with ID: {BookId}", id);
            try
            {
                await _bookService.DeleteBookAsync(id, modifiedById, cancellationToken);
                return NoContent();
            }
            catch (BookNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            // CHANGE: Added catch block for when modifiedById <= 0
            catch (BookValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search/title/{searchTerm}")]
        [ProducesResponseType(typeof(PagedResult<BookDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<BookDTO>>> GetBooksByTitle(
            [FromRoute] string searchTerm,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET request: Search books by title: {SearchTerm}", searchTerm);
            try
            {
                var books = await _bookService.GetByTitleAsync(searchTerm, page, pageSize, cancellationToken);
                return Ok(books);
            }
            catch (BookValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search/author/{searchTerm}")]
        [ProducesResponseType(typeof(PagedResult<BookDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<BookDTO>>> GetBooksByAuthor(
            [FromRoute] string searchTerm,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET request: Search books by author: {SearchTerm}", searchTerm);
            try
            {
                var books = await _bookService.GetByAuthorAsync(searchTerm, page, pageSize, cancellationToken);
                return Ok(books);
            }
            catch (BookValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search/publisher/{searchTerm}")]
        [ProducesResponseType(typeof(PagedResult<BookDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<BookDTO>>> GetBooksByPublisher(
            [FromRoute] string searchTerm,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET request: Search books by publisher: {SearchTerm}", searchTerm);
            try
            {
                var books = await _bookService.GetByPublisherAsync(searchTerm, page, pageSize, cancellationToken);
                return Ok(books);
            }
            catch (BookValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}