using System.Net.Http.Json;
using MiniDashboard.Context.DTO;
using MiniDashboard.Tests.IntegrationTests.Infrastructure;

namespace MiniDashboard.Tests.IntegrationTests.Endpoints;

public class AuthorEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthorEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAuthors_ReturnsSeededData()
    {
        var authors = await _client.GetFromJsonAsync<List<AuthorDto>>("/authors");

        Assert.NotNull(authors);
        Assert.Contains(authors!, a => a.Name == "Ada Lovelace");
    }
}
