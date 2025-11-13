using Microsoft.AspNetCore.Mvc;
using MiniDashboard.Context.DTO;
using MiniDashboard.Context.Interfaces;

namespace MiniDashboard.Api.Controllers
{
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        [Route("authors")]
        public async Task<List<AuthorDto>> GetAuthors()
        {
            return await _authorService.GetAuthorsAsync();
        }
    }
}
