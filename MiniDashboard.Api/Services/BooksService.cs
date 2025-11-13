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
            var books = await _context.Books.ToListAsync();
            return books.ConvertAll(x => new BookDto(x));
        }

        public async Task<List<BookDto>> QueryByNameAsync(string query)
        {
            var books = await _context.Books
                .Where(x => x.Title.Contains(query)
                    || (x.SubTitle != null && x.SubTitle.Contains(query)))
                .ToListAsync();

            return books.ConvertAll(x => new BookDto(x));
        }

        public async Task<BookDto?> CreateBookAsync(BookDto book)
        {
            var entity = book.ToEntity();

            entity = await PopulateAuthorsAndGenres(entity);

            _context.Books.Add(entity);
            await _context.SaveChangesAsync();
            return new BookDto(entity);
        }

        private async Task<Book> PopulateAuthorsAndGenres(Book book)
        {
            var authorNames = book.Authors.Select(x => x.Name);
            var genreNames = book.Genres.Select(x => x.Name);

            var existingAuthors = await _context.Authors.Where(x => authorNames.Contains(x.Name)).ToListAsync();
            var existingGenres = await _context.Genres.Where(x => genreNames.Contains(x.Name)).ToListAsync();

            List<Author> authors = new List<Author>(book.Authors.Count());
            foreach (var author in book.Authors)
            {
                var existingAuthor = existingAuthors.FirstOrDefault(x => x.Name == author.Name);
                authors.Add(existingAuthor ?? author);
            }
            book.Authors = authors;

            List<Genre> genres = new List<Genre>(book.Genres.Count());
            foreach (var genre in book.Genres)
            {
                var existingGenre = existingGenres.FirstOrDefault(x => x.Name == genre.Name);
                genres.Add(existingGenre ?? genre);
            }
            book.Genres = genres;
            return book;
        }

        public async Task<BookDto?> UpdateBookAsync(BookDto book)
        {
            var entity = book.ToEntity();

            entity = await PopulateAuthorsAndGenres(entity);

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
