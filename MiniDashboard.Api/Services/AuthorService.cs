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

        public async Task<AuthorDto?> GetAuthorByIdAsync(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return null;
            }

            return new AuthorDto(author);
        }

        public async Task<List<AuthorDto>> GetAuthorsAsync()
        {
            var authors = await _context.Authors.ToListAsync();
            return authors.ConvertAll(x => new AuthorDto(x));
        }

        public async Task<AuthorDto?> CreateAuthorAsync(AuthorDto author)
        {
            var entity = author.ToEntity();
            _context.Authors.Add(entity);
            await _context.SaveChangesAsync();
            return new AuthorDto(entity);
        }

        public async Task<AuthorDto?> UpdateAuthorAsync(AuthorDto author)
        {
            var entity = author.ToEntity();
            _context.Authors.Update(entity);
            await _context.SaveChangesAsync();
            return new AuthorDto(entity);
        }

        public async Task DeleteAuthorAsync(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }
        }
    }
}
