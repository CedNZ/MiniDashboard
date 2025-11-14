using System.Linq;
using System.Net.Http.Json;
using MiniDashboard.Context.DTO;
using MiniDashboard.Tests.IntegrationTests.Infrastructure;

namespace MiniDashboard.Tests.IntegrationTests.Endpoints;

public class BookEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BookEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetBooks_ReturnsSeededData()
    {
        var books = await _client.GetFromJsonAsync<List<BookDto>>("/books");

        Assert.NotNull(books);
        Assert.Contains(books!, b => b.Title == "Analytical Engines");
    }

    [Fact]
    public async Task PostBooks_CreatesBook()
    {
        var newBook = new BookDto
        {
            Title = "Integration Testing", 
            Authors = [new AuthorDto { Name = "Ada" }],
            Genres = [new GenreDto { Name = "Tech" }],
        };

        var response = await _client.PostAsJsonAsync("/books", newBook);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(created);
        Assert.True(created!.BookId > 0);

        var fetched = await _client.GetFromJsonAsync<BookDto>($"/books/{created.BookId}");
        Assert.NotNull(fetched);
        Assert.Equal("Integration Testing", fetched!.Title);
    }

    [Fact]
    public async Task PutBooks_UpdatesExistingBook()
    {
        var books = await _client.GetFromJsonAsync<List<BookDto>>("/books");
        Assert.NotNull(books);
        var original = Assert.Single(books!.Where(b => b.Title == "Analytical Engines"));

        original.Title = "Analytical Engines - Updated";
        var response = await _client.PutAsJsonAsync($"/books/{original.BookId}", original);
        response.EnsureSuccessStatusCode();

        var updated = await _client.GetFromJsonAsync<BookDto>($"/books/{original.BookId}");
        Assert.NotNull(updated);
        Assert.Equal("Analytical Engines - Updated", updated!.Title);
    }

    [Fact]
    public async Task DeleteBooks_RemovesBook()
    {
        var newBook = new BookDto
        {
            Title = "Disposable", 
            Authors = [new AuthorDto { Name = "Grace" }],
            Genres = [new GenreDto { Name = "History" }],
        };

        var createResponse = await _client.PostAsJsonAsync("/books", newBook);
        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<BookDto>();
        Assert.NotNull(created);

        var deleteResponse = await _client.DeleteAsync($"/books/{created!.BookId}");
        deleteResponse.EnsureSuccessStatusCode();

        var books = await _client.GetFromJsonAsync<List<BookDto>>("/books");
        Assert.NotNull(books);
        Assert.DoesNotContain(books!, b => b.BookId == created.BookId);
    }
}
