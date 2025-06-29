using LetterGeneration.DataAccess;
using LetterGeneration.DataAccess.Models;
using LetterGeneration.Services.Services;
using LetterGeneration.Tests.TestHelpers;
using Xunit;

namespace LetterGeneration.Tests.Services
{
    public class LetterServiceTests : IAsyncLifetime
    {
        private readonly string _connectionString = "Server=localhost;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";
        private readonly DatabaseContext _context;
        private readonly LetterService _service;

        public LetterServiceTests()
        {
            _context = DatabaseTestHelper.CreateTestContext(_connectionString);
            _service = new LetterService(_context);
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
        public async Task CreateLetter_WithValidData_ShouldSucceed()
        {
            // Arrange
            var letter = new Letter
            {
                Title = "Test Letter",
                Content = "Test Content",
                Recipient = "test@example.com"
            };

            // Act
            var id = await _service.CreateLetterAsync(letter);

            // Assert
            Assert.True(id > 0);
            var createdLetter = await _service.GetLetterByIdAsync(id);
            Assert.NotNull(createdLetter);
            Assert.Equal("Draft", createdLetter.Status);
        }

        [Theory]
        [InlineData("", "Content", "recipient@example.com")]
        [InlineData("Title", "", "recipient@example.com")]
        [InlineData("Title", "Content", "")]
        public async Task CreateLetter_WithInvalidData_ShouldThrowArgumentException(string title, string content, string recipient)
        {
            // Arrange
            var letter = new Letter
            {
                Title = title,
                Content = content,
                Recipient = recipient
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateLetterAsync(letter));
        }

        [Fact]
        public async Task SendLetter_ShouldUpdateStatusAndSentDate()
        {
            // Arrange
            var letter = new Letter
            {
                Title = "Test Letter",
                Content = "Test Content",
                Recipient = "test@example.com"
            };
            var id = await _service.CreateLetterAsync(letter);

            // Act
            var success = await _service.SendLetterAsync(id);

            // Assert
            Assert.True(success);
            var sentLetter = await _service.GetLetterByIdAsync(id);
            Assert.Equal("Sent", sentLetter.Status);
            Assert.NotNull(sentLetter.SentDate);
        }

        [Fact]
        public async Task SendLetter_AlreadySent_ShouldThrowException()
        {
            // Arrange
            var letter = new Letter
            {
                Title = "Test Letter",
                Content = "Test Content",
                Recipient = "test@example.com"
            };
            var id = await _service.CreateLetterAsync(letter);
            await _service.SendLetterAsync(id);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.SendLetterAsync(id));
        }

        [Fact]
        public async Task ArchiveLetter_ShouldUpdateStatus()
        {
            // Arrange
            var letter = new Letter
            {
                Title = "Test Letter",
                Content = "Test Content",
                Recipient = "test@example.com"
            };
            var id = await _service.CreateLetterAsync(letter);

            // Act
            var success = await _service.ArchiveLetterAsync(id);

            // Assert
            Assert.True(success);
            var archivedLetter = await _service.GetLetterByIdAsync(id);
            Assert.Equal("Archived", archivedLetter.Status);
        }

        [Fact]
        public async Task ArchiveLetter_AlreadyArchived_ShouldThrowException()
        {
            // Arrange
            var letter = new Letter
            {
                Title = "Test Letter",
                Content = "Test Content",
                Recipient = "test@example.com"
            };
            var id = await _service.CreateLetterAsync(letter);
            await _service.ArchiveLetterAsync(id);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.ArchiveLetterAsync(id));
        }

        [Fact]
        public async Task DeleteLetter_ShouldRemoveLetter()
        {
            // Arrange
            var letter = new Letter
            {
                Title = "Test Letter",
                Content = "Test Content",
                Recipient = "test@example.com"
            };
            var id = await _service.CreateLetterAsync(letter);

            // Act
            var success = await _service.DeleteLetterAsync(id);

            // Assert
            Assert.True(success);
            var deletedLetter = await _service.GetLetterByIdAsync(id);
            Assert.Null(deletedLetter);
        }

        [Fact]
        public async Task DeleteLetter_NonExistentId_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.DeleteLetterAsync(999));
        }
    }
} 