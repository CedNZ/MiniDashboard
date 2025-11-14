using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MiniDashboard.Api;
using MiniDashboard.App.Services;
using MiniDashboard.Context.ApiModels;
using MiniDashboard.Context.DTO;
using MiniDashboard.Tests.UnitTests.TestHelpers;

namespace MiniDashboard.Tests.UnitTests.Services;

public class AppAuthorServiceTests
{
    private readonly List<SampleRequest> _requests;
    private readonly IHttpClientFactory _httpClientFactory;

    public AppAuthorServiceTests()
    {
        _requests = [];
        _httpClientFactory = HttpHelper.GetHttpClientFactory(_requests);
    }

    [Fact]
    public async Task GetAuthorsAsync_ReturnsAuthorDtos()
    {
        //Arrange
        await using var context = CreateContext();
        var author = new Author { Name = "Ada" };
        var book = new Book { Title = "Test Driven" };
        author.Books.Add(book);
        book.Authors.Add(author);

        context.Authors.Add(author);
        await context.SaveChangesAsync();

        _requests.Add(new SampleRequest
        {
            Url = "/authors",
            ResponseContent = new StringContent(JsonSerializer.Serialize(new List<AuthorDto> { new(author) }))
        });

        var service = new AppAuthorsService(_httpClientFactory);

        //Act
        var authors = await service.GetAuthorsAsync();

        //Assert
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
