
using Microsoft.AspNetCore.Mvc;
using MovieManager.API.DTOs.BookDTO;
using MovieManager.API.Services.Interface;

namespace MovieManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BooksController : ControllerBase
    {
        private readonly iBookService _bookService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(iBookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService ?? throw new ArgumentNullException(nameof(bookService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetAllBooks()
        {
            _logger.LogInformation("GET request: Retrieve all books");
            var books = await _bookService.GetAll();
            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDTO>> GetBookById(int id)
        {
            _logger.LogInformation("GET request: Retrieve book by ID: {bookId}", id);
            var book = await _bookService.GetById(id);
            if (book == null)
            {
                _logger.LogWarning("Book with ID {bookId} not found", id);
                return NotFound();
            }
            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult> CreateBook([FromBody] CreateBookDTO book)
        {
            _logger.LogInformation("POST request: Create a new book");
            var createdBook = await _bookService.CreateBook(book);
            return CreatedAtAction(nameof(GetBookById), new { id = createdBook.Id }, createdBook);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateBook(int id, [FromBody] UpdateBookDTO book)
        {
            _logger.LogInformation("PUT request: Update book with ID: {bookId}", id);
            var updatedBook = await _bookService.UpdateBook(id, book);
            if (updatedBook == null)
            {
                _logger.LogWarning("Book with ID {bookId} not found", id);
                return NotFound();
            }
            return Ok(updatedBook);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBook(int id, int modifiedById)
        {
            _logger.LogInformation("DELETE request: Delete book with ID: {bookId}", id);
            var deleted = await _bookService.DeleteBook(id, modifiedById);
            if (!deleted)
            {
                _logger.LogWarning("Book with ID {bookId} not found", id);
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("search/title/{searchTerm}")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooksByTitle([FromRoute] string searchTerm)
        {
            _logger.LogInformation("GET request: Search books with term: {searchTerm}", searchTerm);
            var books = await _bookService.GetByTitle(searchTerm);
            return Ok(books);
        }

        [HttpGet("search/Author/{searchTerm}")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooksByAuthor([FromRoute] string searchTerm)
        {
            _logger.LogInformation("GET request: Search books with term: {searchTerm}", searchTerm);
            var books = await _bookService.GetByAuthor(searchTerm);
            return Ok(books);
        }

        [HttpGet("search/publisher/{searchTerm}")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooksByPublisher([FromRoute] string searchTerm)
        {
            _logger.LogInformation("GET request: Search books with term: {searchTerm}", searchTerm);
            var books = await _bookService.GetByPublisher(searchTerm);
            return Ok(books);
        }
    }
}