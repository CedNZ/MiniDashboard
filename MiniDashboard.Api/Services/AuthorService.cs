using Microsoft.EntityFrameworkCore;
using MiniDashboard.Context.DTO;
using MiniDashboard.Context.Interfaces;

namespace MiniDashboard.Api.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly ApiDbContext _context;

        public AuthorService(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<List<AuthorDto>> GetAuthorsAsync()
        {
            var authors = await _context.Authors
                .Include(x => x.Books)
                .ToListAsync();
            return authors.ConvertAll(x => new AuthorDto(x));
        }
    }
}
