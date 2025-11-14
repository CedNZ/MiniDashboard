using Microsoft.EntityFrameworkCore;
using MiniDashboard.Api;
using MiniDashboard.Api.Services;
using MiniDashboard.Context.ApiModels;
using MiniDashboard.Context.DTO;

namespace MiniDashboard.Tests.UnitTests.Services;

public class BooksServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsBooksWithAuthorsAndGenres()
    {
        await using var context = CreateContext();
        var author = new Author { Name = "Ada" };
        var genre = new Genre { Name = "Education" };
        var book = new Book
        {
            Title = "Learning Testing",
            Authors = { author },
            Genres = { genre },
        };
        author.Books.Add(book);
        genre.Books.Add(book);

        context.Books.Add(book);
        await context.SaveChangesAsync();

        var service = new BooksService(context);

        var results = await service.GetAllAsync();

        var dto = Assert.Single(results);
        Assert.Equal("Learning Testing", dto.Title);
        Assert.Equal("Ada", dto.Authors.Single().Name);
        Assert.Equal("Education", dto.Genres.Single().Name);
    }

    [Fact]
    public async Task QueryByNameAsync_MatchesAcrossFields()
    {
        await using var context = CreateContext();
        context.Books.AddRange(
            new Book { Title = "First", Authors = { new Author { Name = "Ada" } } },
            new Book { Title = "Second", SubTitle = "Mystery", Authors = { new Author { Name = "Bob" } } }
        );
        await context.SaveChangesAsync();

        var service = new BooksService(context);

        var results = await service.QueryByNameAsync("Mystery");

        var dto = Assert.Single(results);
        Assert.Equal("Second", dto.Title);
    }

    [Fact]
    public async Task CreateBookAsync_ReusesExistingAuthorsAndGenres()
    {
        await using var context = CreateContext();
        var existingAuthor = new Author { Name = "Ada" };
        var existingGenre = new Genre { Name = "Tech" };
        context.Authors.Add(existingAuthor);
        context.Genres.Add(existingGenre);
        await context.SaveChangesAsync();

        var service = new BooksService(context);

        var dto = new BookDto
        {
            Title = "New Book",
            Authors = [new AuthorDto { Name = "Ada" }],
            Genres = [new GenreDto { Name = "Tech" }],
        };

        var created = await service.CreateBookAsync(dto);

        Assert.NotNull(created);
        Assert.Equal(1, await context.Authors.CountAsync());
        Assert.Equal(1, await context.Genres.CountAsync());
        Assert.Equal("Ada", created!.Authors.Single().Name);
        Assert.Equal("Tech", created.Genres.Single().Name);
    }

    [Fact]
    public async Task UpdateBookAsync_UpdatesExistingBook()
    {
        await using var context = CreateContext();
        var author = new Author { Name = "Ada" };
        var genre = new Genre { Name = "Tech" };
        var book = new Book
        {
            Title = "Old Title",
            Authors = { author },
            Genres = { genre },
        };
        author.Books.Add(book);
        genre.Books.Add(book);
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var service = new BooksService(context);

        var updatedDto = new BookDto
        {
            BookId = book.BookId,
            Title = "Updated Title",
            Authors = [new AuthorDto { Name = "Bob" }],
            Genres = [new GenreDto { Name = "Science" }],
        };

        var updated = await service.UpdateBookAsync(updatedDto);

        Assert.NotNull(updated);
        Assert.Equal("Updated Title", updated!.Title);
        Assert.Equal("Bob", updated.Authors.Single().Name);
        Assert.Equal("Science", updated.Genres.Single().Name);
        Assert.Equal(1, await context.Books.CountAsync());
    }

    [Fact]
    public async Task DeleteBookAsync_RemovesBook()
    {
        await using var context = CreateContext();
        var book = new Book { Title = "Temp" };
        context.Books.Add(book);
        await context.SaveChangesAsync();

        var service = new BooksService(context);
        await service.DeleteBookAsync(book.BookId);

        Assert.Empty(context.Books);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullWhenMissing()
    {
        await using var context = CreateContext();
        var service = new BooksService(context);

        var result = await service.GetByIdAsync(42);

        Assert.Null(result);
    }

    private static ApiDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApiDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new ApiDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
