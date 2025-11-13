using MiniDashboard.Context.DTO;

namespace MiniDashboard.Context.Interfaces
{
    public interface IAuthorService
    {
        Task<List<AuthorDto>> GetAuthorsAsync();
    }
}
