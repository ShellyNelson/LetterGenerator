using LetterGeneration.DataAccess;
using LetterGeneration.DataAccess.Models;
using LetterGeneration.DataAccess.Repositories;
using LetterGeneration.Tests.TestHelpers;
using Xunit;

namespace LetterGeneration.Tests.Repositories
{
    public class LetterRepositoryTests : IAsyncLifetime
    {
        private readonly string _connectionString = "Server=localhost;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";
        private readonly DatabaseContext _context;
        private readonly LetterRepository _repository;

        public LetterRepositoryTests()
        {
            _context = DatabaseTestHelper.CreateTestContext(_connectionString);
            _repository = new LetterRepository(_context);
        }

        public async Task InitializeAsync()
        {
            await DatabaseTestHelper.InitializeTestDatabase(_connectionString);
        }

        public async Task DisposeAsync()
        {
            await DatabaseTestHelper.CleanupTestDatabase(_connectionString);
        }

        [Fact]
        public async Task CreateLetter_ShouldReturnNewId()
        {
            // Arrange
            var letter = new Letter
            {
                Title = "Test Letter",
                Content = "Test Content",
                Recipient = "test@example.com",
                Status = "Draft"
            };

            // Act
            var id = await _repository.CreateLetterAsync(letter);

            // Assert
            Assert.True(id > 0);
        }

        [Fact]
        public async Task GetLetterById_ShouldReturnCorrectLetter()
        {
            // Arrange
            var letter = new Letter
            {
                Title = "Test Letter",
                Content = "Test Content",
                Recipient = "test@example.com",
                Status = "Draft"
            };
            var id = await _repository.CreateLetterAsync(letter);

            // Act
            var retrievedLetter = await _repository.GetLetterByIdAsync(id);

            // Assert
            Assert.NotNull(retrievedLetter);
            Assert.Equal(letter.Title, retrievedLetter.Title);
            Assert.Equal(letter.Content, retrievedLetter.Content);
            Assert.Equal(letter.Recipient, retrievedLetter.Recipient);
            Assert.Equal(letter.Status, retrievedLetter.Status);
        }

        [Fact]
        public async Task UpdateLetter_ShouldUpdateCorrectly()
        {
            // Arrange
            var letter = new Letter
            {
                Title = "Original Title",
                Content = "Original Content",
                Recipient = "original@example.com",
                Status = "Draft"
            };
            var id = await _repository.CreateLetterAsync(letter);

            // Act
            letter.Id = id;
            letter.Title = "Updated Title";
            letter.Content = "Updated Content";
            var success = await _repository.UpdateLetterAsync(letter);

            // Assert
            Assert.True(success);
            var updatedLetter = await _repository.GetLetterByIdAsync(id);
            Assert.Equal("Updated Title", updatedLetter.Title);
            Assert.Equal("Updated Content", updatedLetter.Content);
        }

        [Fact]
        public async Task DeleteLetter_ShouldRemoveLetter()
        {
            // Arrange
            var letter = new Letter
            {
                Title = "Test Letter",
                Content = "Test Content",
                Recipient = "test@example.com",
                Status = "Draft"
            };
            var id = await _repository.CreateLetterAsync(letter);

            // Act
            var success = await _repository.DeleteLetterAsync(id);

            // Assert
            Assert.True(success);
            var deletedLetter = await _repository.GetLetterByIdAsync(id);
            Assert.Null(deletedLetter);
        }

        [Fact]
        public async Task GetAllLetters_ShouldReturnAllLetters()
        {
            // Arrange
            var letter1 = new Letter
            {
                Title = "Letter 1",
                Content = "Content 1",
                Recipient = "test1@example.com",
                Status = "Draft"
            };
            var letter2 = new Letter
            {
                Title = "Letter 2",
                Content = "Content 2",
                Recipient = "test2@example.com",
                Status = "Draft"
            };
            await _repository.CreateLetterAsync(letter1);
            await _repository.CreateLetterAsync(letter2);

            // Act
            var letters = await _repository.GetAllLettersAsync();

            // Assert
            Assert.Equal(2, letters.Count());
        }
    }
} 