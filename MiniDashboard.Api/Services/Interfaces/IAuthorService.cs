using MiniDashboard.Context.ApiModels;

namespace MiniDashboard.Api.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<Author?> GetAuthorByIdAsync(int id);
        Task<List<Author>> GetAuthorsAsync();
        Task<Author> CreateAuthorAsync(Author author);
        Task<Author> UpdateAuthorAsync(Author author);
        Task DeleteAuthorAsync(int id);
    }
}
