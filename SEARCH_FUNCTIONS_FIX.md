## Search Functions Fix - EF.Functions.Like Implementation

### ❌ Problem
The search functions were using `string.Contains(title, StringComparison.OrdinalIgnoreCase)` which cannot be translated to SQL by Entity Framework Core, resulting in:

```
System.InvalidOperationException: 'The LINQ expression 'DbSet<Movie>()
	.Where(m => m.Title.Contains(
		value: @title, 
		comparisonType: OrdinalIgnoreCase))' could not be translated.
```

### ✅ Solution
Replaced all three search functions to use `EF.Functions.Like()` which is the Entity Framework Core method designed for SQL Server LIKE pattern matching.

### 🔧 Changes Made

#### 1. SearchMoviesByTitleAsync
**Before:**
```csharp
var movies = await _dbContext.Movies
	.Where(m => m.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
	.OrderBy(m => m.Title)
	.ToListAsync();
```

**After:**
```csharp
var searchPattern = $"%{title}%";
var movies = await _dbContext.Movies
	.Where(m => EF.Functions.Like(m.Title, searchPattern))
	.OrderBy(m => m.Title)
	.ToListAsync();
```

#### 2. SearchMoviesByDirectorAsync
**Before:**
```csharp
var movies = await _dbContext.Movies
	.Where(m => m.Director.Contains(director, StringComparison.OrdinalIgnoreCase))
	.OrderBy(m => m.Director)
	.ThenBy(m => m.Title)
	.ToListAsync();
```

**After:**
```csharp
var searchPattern = $"%{director}%";
var movies = await _dbContext.Movies
	.Where(m => EF.Functions.Like(m.Director, searchPattern))
	.OrderBy(m => m.Director)
	.ThenBy(m => m.Title)
	.ToListAsync();
```

#### 3. SearchMoviesByGenreAsync
**Before:**
```csharp
var movies = await _dbContext.Movies
	.Where(m => m.Genre.Contains(genre, StringComparison.OrdinalIgnoreCase))
	.OrderBy(m => m.Genre)
	.ThenBy(m => m.Title)
	.ToListAsync();
```

**After:**
```csharp
var searchPattern = $"%{genre}%";
var movies = await _dbContext.Movies
	.Where(m => EF.Functions.Like(m.Genre, searchPattern))
	.OrderBy(m => m.Genre)
	.ThenBy(m => m.Title)
	.ToListAsync();
```

### 📝 Key Points

- **EF.Functions.Like()** - Entity Framework Core method for SQL LIKE pattern matching
- **Search Pattern** - Uses `%searchTerm%` format for wildcard matching (% = any characters)
- **Case-Insensitive** - SQL Server's LIKE operator is case-insensitive by default
- **SQL Translation** - These queries are now properly translated to SQL:
  ```sql
  WHERE [Title] LIKE '%searchTerm%'
  WHERE [Director] LIKE '%searchTerm%'
  WHERE [Genre] LIKE '%searchTerm%'
  ```

### ✅ Testing
All search functions now work correctly with full SQL translation:
- Search by Title: `/api/movies/search/title/Inception`
- Search by Director: `/api/movies/search/director/Christopher%20Nolan`
- Search by Genre: `/api/movies/search/genre/Drama`

### 🚀 Build Status
✅ Build successful - All code compiles without errors
