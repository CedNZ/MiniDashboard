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

        public async Task<AuthorDto?> GetAuthorByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"authors/{id}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AuthorDto>();
        }

        public async Task<List<AuthorDto>> GetAuthorsAsync()
        {
            var response = await _httpClient.GetAsync("authors");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<AuthorDto>>() ?? [];
        }

        public async Task<AuthorDto?> CreateAuthorAsync(AuthorDto author)
        {
            var response = await _httpClient.PostAsJsonAsync("authors", author);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AuthorDto>();
        }

        public async Task<AuthorDto?> UpdateAuthorAsync(AuthorDto author)
        {
            var response = await _httpClient.PutAsJsonAsync($"authors/{author.AuthorId}", author);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<AuthorDto>();
        }

        public async Task DeleteAuthorAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"authors/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
