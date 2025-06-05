# Letter Generation System

A .NET-based system for managing and generating letters with a clean architecture approach. The system is built using C# and SQL Server, following best practices for separation of concerns and testability.

## Project Structure

The solution consists of three main projects:

1. **LetterGeneration.DataAccess**
   - Handles all database operations
   - Contains models and repositories
   - Uses Microsoft.Data.SqlClient for SQL Server connectivity

2. **LetterGeneration.Services**
   - Contains business logic
   - Implements validation and business rules
   - Uses the DataAccess layer for persistence

3. **LetterGeneration.Tests**
   - Contains unit and integration tests
   - Uses xUnit as the testing framework
   - Includes test helpers for database operations

## Prerequisites

- .NET 8.0 SDK or later
- SQL Server (LocalDB, Express, or full version)
- Visual Studio 2022 or later (recommended)

## Database Setup

1. Create a new SQL Server database named `LetterGeneration`
2. Run the following SQL script to create the required table:

```sql
CREATE TABLE Letters (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    Recipient NVARCHAR(200) NOT NULL,
    CreatedDate DATETIME2 NOT NULL,
    SentDate DATETIME2 NULL,
    Status NVARCHAR(50) NOT NULL
);
```

## Configuration

Update the connection string in your application to point to your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LetterGeneration;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Building and Running

1. Clone the repository
2. Open the solution in Visual Studio
3. Restore NuGet packages
4. Build the solution
5. Run the tests to verify everything is working

```bash
dotnet restore
dotnet build
dotnet test
```

## Usage Examples

### Creating a Letter

```csharp
var connectionString = "your_connection_string";
var context = new DatabaseContext(connectionString);
var letterService = new LetterService(context);

var letter = new Letter
{
    Title = "Meeting Invitation",
    Content = "Please join us for a meeting...",
    Recipient = "john.doe@example.com"
};

var letterId = await letterService.CreateLetterAsync(letter);
```

### Sending a Letter

```csharp
await letterService.SendLetterAsync(letterId);
```

### Archiving a Letter

```csharp
await letterService.ArchiveLetterAsync(letterId);
```

### Retrieving Letters

```csharp
// Get all letters
var allLetters = await letterService.GetAllLettersAsync();

// Get a specific letter
var letter = await letterService.GetLetterByIdAsync(letterId);
```

## Testing

The project includes comprehensive tests for both the repository and service layers. Tests use a separate test database (`LetterGenerationTest`) to avoid affecting production data.

To run the tests:

```bash
dotnet test
```

## Project Architecture

The project follows a clean architecture approach:

- **Data Access Layer**: Handles all database operations
  - Models: Data entities
  - Repositories: Database access logic
  - DatabaseContext: Connection management

- **Service Layer**: Implements business logic
  - Validation
  - Business rules
  - Error handling

- **Test Layer**: Ensures code quality
  - Unit tests
  - Integration tests
  - Test helpers

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details. 