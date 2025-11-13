using Microsoft.EntityFrameworkCore;
using MiniDashboard.Api.Services.Interfaces;
using MiniDashboard.Context.ApiModels;

namespace MiniDashboard.Api.Services
{
    public class BooksService : IBooksService
    {
        private readonly ApiDbContext _context;

        public BooksService(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<List<Book>> QueryByNameAsync(string query)
        {
            return await _context.Books
                .Where(x => x.Title.Contains(query)
                    || (x.SubTitle != null && x.SubTitle.Contains(query)))
                .ToListAsync();
        }

        public async Task<Book> CreateBookAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book> UpdateBookAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await GetByIdAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }
    }
}
