using MiniDashboard.Context.DTO;

namespace MiniDashboard.Context.Interfaces
{
    public interface IAuthorService
    {
        Task<AuthorDto?> GetAuthorByIdAsync(int id);
        Task<List<AuthorDto>> GetAuthorsAsync();
        Task<AuthorDto?> CreateAuthorAsync(AuthorDto author);
        Task<AuthorDto?> UpdateAuthorAsync(AuthorDto author);
        Task DeleteAuthorAsync(int id);
    }
}
