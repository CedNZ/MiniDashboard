using System.Net.Http;
using System.Net.Http.Json;
using MiniDashboard.Context.DTO;
using MiniDashboard.Context.Interfaces;

namespace MiniDashboard.App.Services
{
    public class AppAuthorsService : IAuthorService
    {
        private readonly HttpClient _httpClient;

        public AppAuthorsService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public async Task<List<AuthorDto>> GetAuthorsAsync()
        {
            var response = await _httpClient.GetAsync("authors");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<AuthorDto>>() ?? [];
        }
    }
}
