using LetterGeneration.DataAccess;

namespace LetterGeneration.Services
{
    public abstract class BaseService
    {
        protected readonly DatabaseContext _context;

        protected BaseService(DatabaseContext context)
        {
            _context = context;
        }
    }
} 