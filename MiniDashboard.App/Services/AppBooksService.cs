using System.Net.Http;
using System.Net.Http.Json;
using MiniDashboard.Context.DTO;
using MiniDashboard.Context.Interfaces;

namespace MiniDashboard.App.Services
{
    public class AppBooksService : IBooksService
    {
        private readonly HttpClient _httpClient;

        public AppBooksService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("API");
        }

        public async Task<BookDto?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"books/{id}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<BookDto>();
        }

        public async Task<List<BookDto>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("books");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<BookDto>>() ?? [];
        }

        public async Task<List<BookDto>> QueryByNameAsync(string query)
        {
            var response = await _httpClient.GetAsync($"books/search?query={query}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<BookDto>>() ?? [];
        }

        public async Task<BookDto?> CreateBookAsync(BookDto book)
        {
            var response = await _httpClient.PostAsJsonAsync("books", book);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<BookDto>();
        }

        public async Task<BookDto?> UpdateBookAsync(BookDto book)
        {
            var response = await _httpClient.PutAsJsonAsync($"books/{book.BookId}", book);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<BookDto>();
        }

        public async Task DeleteBookAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"books/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
