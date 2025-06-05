using LetterGeneration.DataAccess.Models;
using LetterGeneration.DataAccess.Repositories;

namespace LetterGeneration.Services.Services
{
    public class LetterService : BaseService
    {
        private readonly LetterRepository _letterRepository;

        public LetterService(DatabaseContext context) : base(context)
        {
            _letterRepository = new LetterRepository(context);
        }

        public async Task<IEnumerable<Letter>> GetAllLettersAsync()
        {
            return await _letterRepository.GetAllLettersAsync();
        }

        public async Task<Letter?> GetLetterByIdAsync(int id)
        {
            return await _letterRepository.GetLetterByIdAsync(id);
        }

        public async Task<int> CreateLetterAsync(Letter letter)
        {
            // Validate letter data
            if (string.IsNullOrWhiteSpace(letter.Title))
                throw new ArgumentException("Letter title cannot be empty", nameof(letter));

            if (string.IsNullOrWhiteSpace(letter.Content))
                throw new ArgumentException("Letter content cannot be empty", nameof(letter));

            if (string.IsNullOrWhiteSpace(letter.Recipient))
                throw new ArgumentException("Letter recipient cannot be empty", nameof(letter));

            // Set default values
            letter.CreatedDate = DateTime.UtcNow;
            letter.Status = "Draft";

            return await _letterRepository.CreateLetterAsync(letter);
        }

        public async Task<bool> UpdateLetterAsync(Letter letter)
        {
            // Validate letter exists
            var existingLetter = await _letterRepository.GetLetterByIdAsync(letter.Id);
            if (existingLetter == null)
                throw new ArgumentException($"Letter with ID {letter.Id} not found", nameof(letter));

            // Validate letter data
            if (string.IsNullOrWhiteSpace(letter.Title))
                throw new ArgumentException("Letter title cannot be empty", nameof(letter));

            if (string.IsNullOrWhiteSpace(letter.Content))
                throw new ArgumentException("Letter content cannot be empty", nameof(letter));

            if (string.IsNullOrWhiteSpace(letter.Recipient))
                throw new ArgumentException("Letter recipient cannot be empty", nameof(letter));

            return await _letterRepository.UpdateLetterAsync(letter);
        }

        public async Task<bool> DeleteLetterAsync(int id)
        {
            // Validate letter exists
            var existingLetter = await _letterRepository.GetLetterByIdAsync(id);
            if (existingLetter == null)
                throw new ArgumentException($"Letter with ID {id} not found", nameof(id));

            return await _letterRepository.DeleteLetterAsync(id);
        }

        public async Task<bool> SendLetterAsync(int id)
        {
            var letter = await _letterRepository.GetLetterByIdAsync(id);
            if (letter == null)
                throw new ArgumentException($"Letter with ID {id} not found", nameof(id));

            if (letter.Status == "Sent")
                throw new InvalidOperationException("Letter has already been sent");

            letter.Status = "Sent";
            letter.SentDate = DateTime.UtcNow;

            return await _letterRepository.UpdateLetterAsync(letter);
        }

        public async Task<bool> ArchiveLetterAsync(int id)
        {
            var letter = await _letterRepository.GetLetterByIdAsync(id);
            if (letter == null)
                throw new ArgumentException($"Letter with ID {id} not found", nameof(id));

            if (letter.Status == "Archived")
                throw new InvalidOperationException("Letter is already archived");

            letter.Status = "Archived";

            return await _letterRepository.UpdateLetterAsync(letter);
        }
    }
} 