using Microsoft.EntityFrameworkCore;
using BlazorAPI.API.DTOs.BookDTO;
using BlazorAPI.API.Exceptions;
using BlazorAPI.API.Models;
using BlazorAPI.API.Persistence;
using BlazorAPI.API.Services.Interface;

namespace BlazorAPI.API.Services.Implementation
{
    public class BookService : iBookService
    {
        private readonly MovieDbContext _context;
        private readonly ILogger<BookService> _logger;
        public BookService(MovieDbContext context, ILogger<BookService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<BookDTO> GetById(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                throw new KeyNotFoundException($"Book with ID {id} not found.");
            }
            return MapBookToDto(book);
        }
        public async Task<IEnumerable<BookDTO>> GetAll()
        {
            try
            {
                var books= await _context.Books
                                  .AsNoTracking()
                                  .ToListAsync();
                return MapBooksToDtos(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all books.");
                throw; 
            }
        }
        public async Task<BookDTO> CreateBook(CreateBookDTO createBookDTO)
        {
            if (createBookDTO == null)
            {
                throw new ArgumentNullException(nameof(createBookDTO), "CreateBookDTO cannot be null.");
            }

            try
            {
                var book=Book.Create(createBookDTO.Title, createBookDTO.Description, createBookDTO.Author, createBookDTO.Publisher, createBookDTO.Price, createBookDTO.CreatedBy);
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return MapBookToDto(book);
            }
            catch (Exception)
            {
                _logger.LogError("Error occurred while creating a new book.");
                throw;
            }
        }
        public async Task<BookDTO> UpdateBook(int id, UpdateBookDTO updateBookDTO)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid book ID.", nameof(id));
            }

            if (updateBookDTO == null)
            {
                throw new ArgumentNullException(nameof(updateBookDTO), "UpdateBookDTO cannot be null.");
            }

            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    throw new KeyNotFoundException($"Book with ID {id} not found.");
                }   
                book.Update(updateBookDTO.Title, updateBookDTO.Description, updateBookDTO.Author, updateBookDTO.Publisher, updateBookDTO.Price);
                await _context.SaveChangesAsync();
                return MapBookToDto(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating book with ID: {bookId}", id);
                throw;
            }

        }
        public async Task<bool> DeleteBook(int id, int modifiedById)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid book ID.", nameof(id));
            }
            if (modifiedById <= 0) throw new MovieValidationException("ModifiedBy ID must be greater than zero.");

            try
            {
                var book= await _context.Books.FindAsync(id);
                if (book == null)
                {
                    throw new KeyNotFoundException($"Book with ID {id} not found.");
                }
                book.MarkAsDeleted(modifiedById);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                _logger.LogError("Error occurred while deleting book with ID: {bookId}", id);
                throw;
            }
        }
        public Task<IEnumerable<BookDTO>> GetByAuthor(string author)
        {
            if(!string.IsNullOrEmpty(author))
            {
                var searchPattern = $"%{author.ToLower()}%";
                var books = _context.Books
                                   .AsNoTracking()
                                   .Where(b => EF.Functions.Like(b.Author.ToLower(), searchPattern))
                                   .OrderBy(b => b.Title)
                                   .ToList();
                if (books != null)
                {
                    return Task.FromResult(books.Select(MapBookToDto));
                }
            }
            throw new KeyNotFoundException($"Book with author {author} not found.");
        }
        public Task<IEnumerable<BookDTO>> GetByPublisher(string publisher)
        {
            if (!string.IsNullOrEmpty(publisher))
            {
                var searchPattern = $"%{publisher.ToLower()}%";
                var books = _context.Books
                                   .AsNoTracking()
                                   .Where(b => EF.Functions.Like(b.Publisher.ToLower(), searchPattern))
                                   .OrderBy(b => b.Title)
                                   .ToList();
                if (books != null)
                {
                    return Task.FromResult(books.Select(MapBookToDto));
                }
            }
            throw new KeyNotFoundException($"Book with publisher {publisher} not found.");
        }
        public Task<IEnumerable<BookDTO>> GetByTitle(string title)
        {
            if (!string.IsNullOrEmpty(title))
            {
                var searchPattern = $"%{title.ToLower()}%";
                var books = _context.Books
                                   .AsNoTracking()
                                   .Where(b => EF.Functions.Like(b.Title.ToLower(), searchPattern))
                                   .OrderBy(b => b.Title)
                                   .ToList();
                if (books != null)
                {
                    return Task.FromResult(books.Select(MapBookToDto));
                }
            }
            throw new KeyNotFoundException($"Book with title {title} not found.");
        }

        private BookDTO MapBookToDto(Book book)
        {
            return new BookDTO
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Author = book.Author,
                Publisher = book.Publisher,
                Price = book.Price,
                CreatedDate = book.CreatedDate,
                ModifiedDate = book.ModifiedDate,
                CreatedBy = book.CreatedByUser?.Id ?? 0,
                ModifiedBy = book.ModifiedByUser?.Id ?? 0
            };
        }

        private IEnumerable<BookDTO> MapBooksToDtos(IEnumerable<Book> books)
        {
            return books.Select(MapBookToDto);
        }
    }
}
