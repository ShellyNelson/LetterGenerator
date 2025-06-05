using System.Data;
using Microsoft.Data.SqlClient;

namespace LetterGeneration.DataAccess
{
    public abstract class BaseRepository
    {
        protected readonly DatabaseContext _context;

        protected BaseRepository(DatabaseContext context)
        {
            _context = context;
        }

        protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null)
        {
            using var connection = _context.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            if (parameters != null)
            {
                foreach (var prop in parameters.GetType().GetProperties())
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = $"@{prop.Name}";
                    parameter.Value = prop.GetValue(parameters) ?? DBNull.Value;
                    command.Parameters.Add(parameter);
                }
            }

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            var results = new List<T>();
            
            while (await reader.ReadAsync())
            {
                var item = Activator.CreateInstance<T>();
                foreach (var prop in typeof(T).GetProperties())
                {
                    if (reader[prop.Name] != DBNull.Value)
                    {
                        prop.SetValue(item, reader[prop.Name]);
                    }
                }
                results.Add(item);
            }

            return results;
        }

        protected async Task<int> ExecuteAsync(string sql, object? parameters = null)
        {
            using var connection = _context.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            if (parameters != null)
            {
                foreach (var prop in parameters.GetType().GetProperties())
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = $"@{prop.Name}";
                    parameter.Value = prop.GetValue(parameters) ?? DBNull.Value;
                    command.Parameters.Add(parameter);
                }
            }

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }
    }
} 