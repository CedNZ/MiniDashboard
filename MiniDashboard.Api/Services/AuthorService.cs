using Microsoft.EntityFrameworkCore;
using MiniDashboard.Api.Services.Interfaces;
using MiniDashboard.Context.ApiModels;

namespace MiniDashboard.Api.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly ApiDbContext _context;

        public AuthorService(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<Author?> GetAuthorByIdAsync(int id)
        {
            return await _context.Authors.FindAsync(id);
        }

        public async Task<List<Author>> GetAuthorsAsync()
        {
            return await _context.Authors.ToListAsync();
        }

        public async Task<Author> CreateAuthorAsync(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task<Author> UpdateAuthorAsync(Author author)
        {
            _context.Authors.Update(author);
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task DeleteAuthorAsync(int id)
        {
            var author = await GetAuthorByIdAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }
        }
    }
}
