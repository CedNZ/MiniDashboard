using Microsoft.EntityFrameworkCore;
using MiniDashboard.Api;
using MiniDashboard.Api.Services;
using MiniDashboard.Context.ApiModels;

namespace MiniDashboard.Tests.UnitTests.Services;

public class AuthorServiceTests
{
    [Fact]
    public async Task GetAuthorsAsync_ReturnsAuthorDtos()
    {
        await using var context = CreateContext();
        var author = new Author { Name = "Ada" };
        var book = new Book { Title = "Test Driven" };
        author.Books.Add(book);
        book.Authors.Add(author);

        context.Authors.Add(author);
        await context.SaveChangesAsync();

        var service = new AuthorService(context);

        var authors = await service.GetAuthorsAsync();

        var dto = Assert.Single(authors);
        Assert.Equal("Ada", dto.Name);
        Assert.Equal(1, dto.BookCount);
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
