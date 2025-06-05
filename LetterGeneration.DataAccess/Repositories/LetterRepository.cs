using LetterGeneration.DataAccess.Models;

namespace LetterGeneration.DataAccess.Repositories
{
    public class LetterRepository : BaseRepository
    {
        public LetterRepository(DatabaseContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Letter>> GetAllLettersAsync()
        {
            const string sql = "SELECT * FROM Letters ORDER BY CreatedDate DESC";
            return await QueryAsync<Letter>(sql);
        }

        public async Task<Letter?> GetLetterByIdAsync(int id)
        {
            const string sql = "SELECT * FROM Letters WHERE Id = @Id";
            var result = await QueryAsync<Letter>(sql, new { Id = id });
            return result.FirstOrDefault();
        }

        public async Task<int> CreateLetterAsync(Letter letter)
        {
            const string sql = @"
                INSERT INTO Letters (Title, Content, Recipient, CreatedDate, Status)
                VALUES (@Title, @Content, @Recipient, @CreatedDate, @Status);
                SELECT CAST(SCOPE_IDENTITY() as int)";

            using var connection = _context.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = System.Data.CommandType.Text;

            command.Parameters.AddWithValue("@Title", letter.Title);
            command.Parameters.AddWithValue("@Content", letter.Content);
            command.Parameters.AddWithValue("@Recipient", letter.Recipient);
            command.Parameters.AddWithValue("@CreatedDate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@Status", letter.Status);

            await connection.OpenAsync();
            return (int)await command.ExecuteScalarAsync();
        }

        public async Task<bool> UpdateLetterAsync(Letter letter)
        {
            const string sql = @"
                UPDATE Letters 
                SET Title = @Title,
                    Content = @Content,
                    Recipient = @Recipient,
                    Status = @Status,
                    SentDate = @SentDate
                WHERE Id = @Id";

            var rowsAffected = await ExecuteAsync(sql, letter);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteLetterAsync(int id)
        {
            const string sql = "DELETE FROM Letters WHERE Id = @Id";
            var rowsAffected = await ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
    }
} 