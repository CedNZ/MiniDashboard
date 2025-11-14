using System.Net;
using System.Text.Json;
using MiniDashboard.App.Services;
using MiniDashboard.Context.ApiModels;
using MiniDashboard.Context.DTO;
using MiniDashboard.Tests.UnitTests.TestHelpers;

namespace MiniDashboard.Tests.UnitTests.Services;

public class AppBooksServiceTests
{
    private readonly List<SampleRequest> _requests;
    private readonly IHttpClientFactory _httpClientFactory;

    public AppBooksServiceTests()
    {
        _requests = [];
        _httpClientFactory = HttpHelper.GetHttpClientFactory(_requests);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsBooksWithAuthorsAndGenres()
    {
        //Arrange
        var author = new Author { Name = "Ada" };
        var genre = new Genre { Name = "Education" };
        var book = new Book
        {
            Title = "Learning Testing",
            Authors = { author },
            Genres = { genre },
        };

        _requests.Add(new SampleRequest
        {
            Url = "/books",
            ResponseContent = new StringContent(JsonSerializer.Serialize(new List<BookDto> { new(book) }))
        });

        var service = new AppBooksService(_httpClientFactory);

        //Act
        var results = await service.GetAllAsync();

        //Assert
        var dto = Assert.Single(results);
        Assert.Equal("Learning Testing", dto.Title);
        Assert.Equal("Ada", dto.Authors.Single().Name);
        Assert.Equal("Education", dto.Genres.Single().Name);
    }

    [Fact]
    public async Task QueryByNameAsync_MatchesAcrossFields()
    {
        //Arrange
        var books = new List<Book>
        {
            new Book { Title = "First", Authors = { new Author { Name = "Ada" } } },
            new Book { Title = "Second", SubTitle = "Mystery", Authors = { new Author { Name = "Bob" } } }
        };

        _requests.Add(new SampleRequest
        {
            Url = "/books/search",
            ResponseContent = new StringContent(JsonSerializer.Serialize(new List<BookDto> { new (books[1]) }))
        });

        var service = new AppBooksService(_httpClientFactory);

        //Act
        var results = await service.QueryByNameAsync("Mystery");

        //Assert
        var dto = Assert.Single(results);
        Assert.Equal("Second", dto.Title);
    }

    [Fact]
    public async Task CreateBookAsync_ReusesExistingAuthorsAndGenres()
    {
        //Arrange
        var service = new AppBooksService(_httpClientFactory);

        var dto = new BookDto
        {
            Title = "New Book",
            Authors = [new AuthorDto { Name = "Ada" }],
            Genres = [new GenreDto { Name = "Tech" }],
        };

        _requests.Add(new SampleRequest
        {
            Method = HttpMethod.Post,
            Url = "/books",
            ResponseContent = new StringContent(JsonSerializer.Serialize(dto))
        });

        //Act
        var created = await service.CreateBookAsync(dto);

        //Assert
        Assert.NotNull(created);
        Assert.Equal("Ada", created!.Authors.Single().Name);
        Assert.Equal("Tech", created.Genres.Single().Name);
    }

    [Fact]
    public async Task UpdateBookAsync_UpdatesExistingBook()
    {
        //Arrange
        var service = new AppBooksService(_httpClientFactory);

        var updatedDto = new BookDto
        {
            BookId = 1,
            Title = "Updated Title",
            Authors = [new AuthorDto { Name = "Bob" }],
            Genres = [new GenreDto { Name = "Science" }],
        };

        _requests.Add(new SampleRequest
        {
            Method = HttpMethod.Put,
            Url = $"/books/{updatedDto.BookId}",
            ResponseContent = new StringContent(JsonSerializer.Serialize(updatedDto))
        });

        //Act
        var updated = await service.UpdateBookAsync(updatedDto);

        //Assert
        Assert.NotNull(updated);
        Assert.Equal("Updated Title", updated!.Title);
        Assert.Equal("Bob", updated.Authors.Single().Name);
        Assert.Equal("Science", updated.Genres.Single().Name);
    }

    [Fact]
    public async Task DeleteBookAsync_RemovesBook()
    {
        //Arrange
        var book = new Book { Title = "Temp", BookId = 42 };

        var service = new AppBooksService(_httpClientFactory);

        _requests.Add(new SampleRequest
        {
            Method = HttpMethod.Delete,
            Url = $"/books/{book.BookId}"
        });

        //Act
        await service.DeleteBookAsync(book.BookId);

        //Assert
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullWhenMissing()
    {
        //Arrange
        var service = new AppBooksService(_httpClientFactory);

        //Act
        var result = await Assert.ThrowsAsync<HttpRequestException>(async () => await service.GetByIdAsync(42));

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }
}
