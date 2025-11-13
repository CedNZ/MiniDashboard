using MiniDashboard.Context.ApiModels;

namespace MiniDashboard.Api.Services.Interfaces
{
    public interface IBooksService
    {
        Task<Book?> GetByIdAsync(int id);
        Task<List<Book>> GetAllAsync();
        Task<List<Book>> QueryByNameAsync(string query);
        Task<Book> CreateBookAsync(Book book);
        Task<Book> UpdateBookAsync(Book book);
        Task DeleteBookAsync(int id);
    }
}
