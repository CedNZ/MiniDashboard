using MiniDashboard.Context.DTO;

namespace MiniDashboard.Context.Interfaces
{
    public interface IBooksService
    {
        Task<BookDto?> GetByIdAsync(int id);
        Task<List<BookDto>> GetAllAsync();
        Task<List<BookDto>> QueryByNameAsync(string query);
        Task<BookDto?> CreateBookAsync(BookDto book);
        Task<BookDto?> UpdateBookAsync(BookDto book);
        Task DeleteBookAsync(int id);
    }
}
