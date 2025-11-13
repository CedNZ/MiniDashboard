using Microsoft.EntityFrameworkCore;
using MiniDashboard.Context.ApiModels;
using MiniDashboard.Context.DTO;
using MiniDashboard.Context.Interfaces;

namespace MiniDashboard.Api.Services
{
    public class BooksService : IBooksService
    {
        private readonly ApiDbContext _context;

        public BooksService(ApiDbContext context)
        {
            _context = context;
        }

        public async Task<BookDto?> GetByIdAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return null;
            }
            return new BookDto(book);
        }

        public async Task<List<BookDto>> GetAllAsync()
        {
            var books = await _context.Books
                .Include(x => x.Authors)
                .Include(x => x.Genres)
                .ToListAsync();
            return books.ConvertAll(x => new BookDto(x));
        }

        public async Task<List<BookDto>> QueryByNameAsync(string query)
        {
            var books = await _context.Books
                .Include(x => x.Authors)
                .Include(x => x.Genres)
                .Where(x => x.Title.Contains(query)
                    || (x.SubTitle != null && x.SubTitle.Contains(query)))
                .ToListAsync();

            return books.ConvertAll(x => new BookDto(x));
        }

        public async Task<BookDto?> CreateBookAsync(BookDto book)
        {
            var entity = book.ToEntity();

            entity = await PopulateAuthorsAndGenres(entity, book.Authors.ConvertAll(x => x.Name.Trim()), book.Genres.ConvertAll(x => x.Name.Trim()));

            _context.Books.Add(entity);
            await _context.SaveChangesAsync();
            return new BookDto(entity);
        }

        private async Task<Book> PopulateAuthorsAndGenres(Book book, List<string> authors, List<string> genres)
        {
            var existingAuthors = await _context.Authors.Where(x => authors.Contains(x.Name)).ToListAsync();
            var existingGenres = await _context.Genres.Where(x => genres.Contains(x.Name)).ToListAsync();

            book.Authors.Clear();
            foreach (var author in authors)
            {
                var existingAuthor = existingAuthors.FirstOrDefault(x => x.Name == author);
                book.Authors.Add(existingAuthor ?? new Author
                {
                    Name = author,
                });
            }

            book.Genres.Clear();
            foreach (var genre in genres)
            {
                var existingGenre = existingGenres.FirstOrDefault(x => x.Name == genre);
                book.Genres.Add(existingGenre ?? new Genre
                {
                    Name = genre
                });
            }

            return book;
        }

        public async Task<BookDto?> UpdateBookAsync(BookDto book)
        {
            var entity = await _context.Books
                .Include(x => x.Authors)
                .Include(x => x.Genres)
                .FirstOrDefaultAsync(x => x.BookId == book.BookId);

            if (entity == null)
            {
                return null;
            }

            entity.Title = book.Title;
            entity.SubTitle = book.SubTitle;
            entity.Series = book.Series;
            entity.SeriesNumber = book.SeriesNumber;

            entity = await PopulateAuthorsAndGenres(entity, book.Authors.ConvertAll(x => x.Name.Trim()), book.Genres.ConvertAll(x => x.Name.Trim()));

            _context.Books.Update(entity);
            await _context.SaveChangesAsync();
            return new BookDto(entity);
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }
    }
}
