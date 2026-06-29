using Microsoft.EntityFrameworkCore;
using BlazorAPI.API.DTOs.BookDTO;
using BlazorAPI.API.Exceptions;
using BlazorAPI.API.Models;
using BlazorAPI.API.Persistence;
using BlazorAPI.API.Services.Interface;
using BlazorAPI.API.Wrapper;

namespace BlazorAPI.API.Services.Implementation
{
    public class BookService : IBookService
    {
        private readonly MovieDbContext _context;
        private readonly ILogger<BookService> _logger;

        public BookService(MovieDbContext context, ILogger<BookService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PagedResult<BookDTO>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                var baseQuery = _context.Books
                    .AsNoTracking()
                    .Where(b => !(b.IsDeleted ?? false));

                var totalCount = await baseQuery.CountAsync(cancellationToken);

                var items = await baseQuery
                    .OrderByDescending(b => b.CreatedDate)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(b => MapToDto(b))
                    .ToListAsync(cancellationToken);

                return new PagedResult<BookDTO>
                {
                    Items = items,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving books for page {Page}.", page);
                throw new BookException("Failed to retrieve books. Please try again later.", 500);
            }
        }

        public async Task<BookDTO> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            GuardPositiveId(id, "Book");

            try
            {
                var book = await _context.Books
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == id && !(b.IsDeleted ?? false), cancellationToken);

                if (book is null) throw new BookNotFoundException(id);

                return MapToDto(book);
            }
            catch (BookNotFoundException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving book with ID {BookId}.", id);
                throw new BookException("Failed to retrieve book.", 500);
            }
        }

        public async Task<BookDTO> CreateBookAsync(CreateBookDTO createBookDTO, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(createBookDTO);

            try
            {
                var book = Book.Create(
                    createBookDTO.Title,
                    createBookDTO.Description,
                    createBookDTO.Author,
                    createBookDTO.Publisher,
                    createBookDTO.Price,
                    createBookDTO.CreatedBy
                );

                _context.Books.Add(book);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Book created with ID {BookId}.", book.Id);
                return MapToDto(book);
            }
            catch (ArgumentException ex)
            {
                throw new BookValidationException($"Book validation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating book.");
                throw new BookException("Failed to create book.", 500);
            }
        }

        public async Task<BookDTO> UpdateBookAsync(int id, UpdateBookDTO updateBookDTO, CancellationToken cancellationToken = default)
        {
            GuardPositiveId(id, "Book");
            ArgumentNullException.ThrowIfNull(updateBookDTO);

            try
            {
                var book = await _context.Books
                    .FirstOrDefaultAsync(b => b.Id == id && !(b.IsDeleted ?? false), cancellationToken);

                if (book is null) throw new BookNotFoundException(id);

                book.Update(
                    updateBookDTO.Title,
                    updateBookDTO.Description,
                    updateBookDTO.Author,
                    updateBookDTO.Publisher,
                    updateBookDTO.Price
                );

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Book with ID {BookId} updated.", id);
                return MapToDto(book);
            }
            catch (BookNotFoundException) { throw; }
            catch (ArgumentException ex)
            {
                throw new BookValidationException($"Book validation failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating book with ID {BookId}.", id);
                throw new BookException("Failed to update book.", 500);
            }
        }

        public async Task DeleteBookAsync(int id, int modifiedById, CancellationToken cancellationToken = default)
        {
            GuardPositiveId(id, "Book");
            GuardPositiveId(modifiedById, "ModifiedBy");

            try
            {
                var book = await _context.Books
                    .FirstOrDefaultAsync(b => b.Id == id && !(b.IsDeleted ?? false), cancellationToken);

                if (book is null) throw new BookNotFoundException(id);

                book.MarkAsDeleted(modifiedById);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Book with ID {BookId} soft-deleted by user {UserId}.", id, modifiedById);
            }
            catch (BookNotFoundException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting book with ID {BookId}.", id);
                throw new BookException("Failed to delete book.", 500);
            }
        }

        public async Task<PagedResult<BookDTO>> GetByTitleAsync(string title, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new BookValidationException("Title search cannot be empty.");

            return await SearchBooksAsync(
                predicate: b => EF.Functions.Like(b.Title, $"%{title}%"),
                orderBy: q => q.OrderBy(b => b.Title),
                page, pageSize, cancellationToken
            );
        }

        public async Task<PagedResult<BookDTO>> GetByAuthorAsync(string author, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(author)) throw new BookValidationException("Author search cannot be empty.");

            return await SearchBooksAsync(
                predicate: b => EF.Functions.Like(b.Author, $"%{author}%"),
                orderBy: q => q.OrderBy(b => b.Author).ThenBy(b => b.Title),
                page, pageSize, cancellationToken
            );
        }

        public async Task<PagedResult<BookDTO>> GetByPublisherAsync(string publisher, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(publisher)) throw new BookValidationException("Publisher search cannot be empty.");

            return await SearchBooksAsync(
                predicate: b => EF.Functions.Like(b.Publisher, $"%{publisher}%"),
                orderBy: q => q.OrderBy(b => b.Publisher).ThenBy(b => b.Title),
                page, pageSize, cancellationToken
            );
        }

        // ---------------------------------------------------------------------------
        // Private helpers
        // ---------------------------------------------------------------------------

        private async Task<PagedResult<BookDTO>> SearchBooksAsync(
            System.Linq.Expressions.Expression<Func<Book, bool>> predicate,
            Func<IQueryable<Book>, IOrderedQueryable<Book>> orderBy,
            int page,
            int pageSize,
            CancellationToken cancellationToken)
        {
            try
            {
                var baseQuery = _context.Books
                    .AsNoTracking()
                    .Where(b => !(b.IsDeleted ?? false))
                    .Where(predicate);

                var totalCount = await baseQuery.CountAsync(cancellationToken);

                var items = await orderBy(baseQuery)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(b => MapToDto(b))
                    .ToListAsync(cancellationToken);

                return new PagedResult<BookDTO>
                {
                    Items = items,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching books for page {Page}.", page);
                throw new BookException("Failed to search books.", 500);
            }
        }

        private static BookDTO MapToDto(Book book) => new()
        {
            Id = book.Id,
            Title = book.Title,
            Description = book.Description,
            Author = book.Author,
            Publisher = book.Publisher,
            Price = book.Price,
            CreatedDate = book.CreatedDate,
            ModifiedDate = book.ModifiedDate,
            CreatedBy = book.CreatedBy ?? 0,
            ModifiedBy = book.ModifiedBy ?? 0
        };

        private static void GuardPositiveId(int id, string paramName)
        {
            if (id <= 0) throw new BookValidationException($"{paramName} ID must be greater than zero.");
        }
    }
}