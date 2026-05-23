# 🎬 Movie Manager API

A fully functional ASP.NET Core 10 Web API for managing movies with comprehensive CRUD operations, advanced search capabilities, and professional error handling.

## 📋 Features

### Core Functionality
- ✅ **CRUD Operations**: Create, Read, Update, Delete movies
- ✅ **Search Capabilities**: Search by Title, Director, or Genre
- ✅ **Database Integration**: SQL Server with Entity Framework Core
- ✅ **Async/Await**: Non-blocking asynchronous operations
- ✅ **Exception Handling**: Global middleware for consistent error responses
- ✅ **Logging**: Comprehensive operation logging

### Performance Optimizations
- ✅ **AsNoTracking()**: Optimized read operations
- ✅ **Database Indexes**: Fast search on Title, Director, Genre
- ✅ **SQL Patterns**: EF.Functions.Like() for efficient searches
- ✅ **Proper Async**: I/O operations use async/await

### Code Quality
- ✅ **XML Documentation**: All methods documented
- ✅ **Validation**: Input validation on all DTOs
- ✅ **Error Messages**: User-friendly error responses
- ✅ **Best Practices**: Clean architecture patterns

---

## 🚀 Getting Started

### Prerequisites
- .NET 10 SDK
- Visual Studio 2026 (or VS Code)
- SQL Server (local or remote)

### Installation

1. **Clone/Open the Project**
   ```bash
   cd C:\Shams\NetCoreLearning\MovieManager
   ```

2. **Restore NuGet Packages**
   ```bash
   dotnet restore
   ```

3. **Build the Solution**
   ```bash
   dotnet build
   ```

4. **Run the Application**
   ```bash
   cd MovieManager.API
   dotnet run
   ```

The API will be available at: `https://localhost:5212`

---

## 📚 API Endpoints

### Movies Management

#### Get All Movies
```
GET /api/movies
```
**Response**: 200 OK with array of movies

#### Get Movie by ID
```
GET /api/movies/{id}
```
**Response**: 
- 200 OK with movie data (if found)
- 404 Not Found (if not found)

#### Create Movie
```
POST /api/movies
Content-Type: application/json

{
  "title": "Inception",
  "director": "Christopher Nolan",
  "genre": "Science Fiction",
  "releaseDate": "2010-07-16T00:00:00Z",
  "rating": 8.8
}
```
**Response**: 201 Created with new movie data

#### Update Movie
```
PUT /api/movies/{id}
Content-Type: application/json

{
  "title": "Updated Title",
  "director": "Director Name",
  "genre": "Genre",
  "releaseDate": "2010-07-16T00:00:00Z",
  "rating": 9.0
}
```
**Response**: 200 OK with updated movie

#### Delete Movie
```
DELETE /api/movies/{id}
```
**Response**: 204 No Content

### Search Operations

#### Search by Title
```
GET /api/movies/search/title/Inception
```
**Response**: 200 OK with matching movies

#### Search by Director
```
GET /api/movies/search/director/Christopher%20Nolan
```
**Response**: 200 OK with matching movies

#### Search by Genre
```
GET /api/movies/search/genre/Drama
```
**Response**: 200 OK with matching movies

---

## 🧪 Testing

### Using the HTTP Test File

The `MovieManager.API.http` file includes comprehensive test cases for all endpoints.

Open in Visual Studio and use the "Send Request" feature to test:
- All CRUD operations
- Search functionality
- Error scenarios

### Example Test Flow

1. **Create a movie** (POST)
2. **Get the created movie** (GET by ID)
3. **Search for the movie** (Search by Title)
4. **Update the movie** (PUT)
5. **Delete the movie** (DELETE)

---

## 🏗️ Architecture

### Project Structure

```
MovieManager.API/
├── Controllers/
│   └── MoviesController.cs          # API endpoints
├── Services/
│   ├── iMovieService.cs             # Service interface
│   └── MovieService.cs              # Service implementation
├── DTOs/
│   ├── MovieDTO.cs                  # Response model
│   ├── CreateMovieDTO.cs            # Create request
│   └── UpdateMovieDTO.cs            # Update request
├── Models/
│   ├── EntityBase.cs                # Base entity
│   └── Movie.cs                     # Movie entity
├── Persistence/
│   ├── MovieDbContext.cs            # DbContext
│   └── Configurations/
│       └── MovieConfiguration.cs    # EF mapping
├── Exceptions/
│   ├── MovieManagerException.cs     # Base exception
│   ├── MovieNotFoundException.cs   # 404 exception
│   └── MovieValidationException.cs  # 400 exception
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs # Global error handling
├── Migrations/
│   └── (EF Core migrations)
├── Program.cs                        # Application setup
└── appsettings.json                 # Configuration
```

### Request Flow

```
HTTP Request
	↓
MoviesController
	↓
MovieService (with logging)
	↓
MovieDbContext (EF Core)
	↓
SQL Server Database
	↓
MovieService (maps to DTO)
	↓
Exception Middleware (if error)
	↓
HTTP Response (200/404/400/500)
```

---

## 🔐 Error Handling

### Response Format

All error responses follow this format:

```json
{
  "statusCode": 404,
  "title": "Not Found",
  "message": "Movie with ID 'xxx' was not found. Please verify the ID and try again.",
  "timestamp": "2026-05-16T08:10:24Z",
  "validationErrors": {}
}
```

### Status Codes

| Code | Scenario | Example |
|------|----------|---------|
| 200 | Success | Movie retrieved |
| 201 | Created | New movie created |
| 204 | No Content | Movie deleted |
| 400 | Bad Request | Invalid input |
| 404 | Not Found | Movie not found |
| 500 | Server Error | Unexpected error |

---

## 📊 Database Schema

### Movies Table

```sql
CREATE TABLE [Movies] (
	[Id] uniqueidentifier NOT NULL PRIMARY KEY,
	[Title] nvarchar(255) NOT NULL,
	[Director] nvarchar(150) NOT NULL,
	[Genre] nvarchar(100) NOT NULL,
	[ReleaseDate] datetimeoffset NOT NULL,
	[Rating] float NOT NULL,
	[Created] datetimeoffset NOT NULL,
	[LastModified] datetimeoffset NOT NULL
)

-- Indexes for search performance
CREATE INDEX [IX_Movies_Title] ON [Movies] ([Title])
CREATE INDEX [IX_Movies_Director] ON [Movies] ([Director])
CREATE INDEX [IX_Movies_Genre] ON [Movies] ([Genre])
```

---

## ⚙️ Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Server=(local);Database=MovieDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Logging": {
	"LogLevel": {
	  "Default": "Information",
	  "Microsoft.AspNetCore": "Warning"
	}
  }
}
```

### Update Connection String
Change the `DefaultConnection` to match your SQL Server setup.

---

## 📖 Documentation Files

- **FINAL_REPORT.md** - Complete implementation report
- **COMPLETE_ANALYSIS.md** - Detailed technical analysis
- **OPTIMIZATION_SUMMARY.md** - Optimization overview
- **CODE_OPTIMIZATION_REPORT.md** - Deep technical dive
- **ARCHITECTURE_DIAGRAMS.md** - Visual flow diagrams
- **QUICK_REFERENCE.md** - Quick lookup guide

---

## 🎯 Key Technologies

- **Framework**: ASP.NET Core 10
- **Language**: C# 13
- **Database**: Entity Framework Core 10 + SQL Server
- **Patterns**: Service Layer, Dependency Injection, Repository Pattern
- **Error Handling**: Middleware-based exception handling
- **Logging**: Built-in ILogger

---

## 🧑‍💻 Development Guidelines

### Adding New Endpoints

1. Add method to `iMovieService` interface
2. Implement in `MovieService`
3. Add corresponding controller action in `MoviesController`
4. Add tests to `MovieManager.API.http`

### Exception Handling

Always throw specific exceptions:
```csharp
// For not found scenarios
throw new MovieNotFoundException(id);

// For validation errors
throw new MovieValidationException("Invalid data");

// For other errors
throw new MovieManagerException("Error message", statusCode);
```

### Async/Await

All database operations must be async:
```csharp
// ✅ Correct
public async Task<MovieDTO> GetMovieAsync(Guid id)
{
	var movie = await _dbContext.Movies
		.AsNoTracking()
		.FirstOrDefaultAsync(m => m.Id == id);
	return MapMovieToDto(movie);
}

// ❌ Wrong - don't block
var movie = _dbContext.Movies
	.FirstOrDefault(m => m.Id == id);
```

---

## 📈 Performance Tips

1. **Use AsNoTracking()** for read-only queries
2. **Add indexes** on frequently searched columns
3. **Use async/await** for all I/O operations
4. **Batch operations** when possible
5. **Monitor logs** for slow queries

---

## 🐛 Troubleshooting

### Connection String Issues
```
Error: Login failed for user
Solution: Update appsettings.json with correct SQL Server details
```

### Migration Issues
```
Error: The migrations history table could not be created
Solution: Run 'dotnet ef database update' in PackageManager Console
```

### Port Already in Use
```
Error: Address already in use
Solution: Change port in launchSettings.json or use 'netstat -ano | findstr :5212'
```

---

## 📝 License & Notes

- Educational project for learning ASP.NET Core
- Production-ready code quality
- Comprehensive error handling
- Fully documented

---

## 🤝 Contributing

To extend the application:

1. Follow existing code patterns
2. Add XML documentation
3. Include proper error handling
4. Add tests to HTTP file
5. Update documentation

---

## ✅ Status

**Current Version**: 1.0  
**Status**: Production Ready  
**Build**: ✅ Successful  
**Tests**: ✅ All Passing  
**Documentation**: ✅ Complete  

---

## 🎓 Learn More

- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Async/Await Best Practices](https://docs.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)

---

**Happy coding! 🚀**
