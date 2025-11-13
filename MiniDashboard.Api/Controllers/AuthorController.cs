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

        [HttpGet]
        [Route("authors/{id:int}")]
        public async Task<AuthorDto?> GetAuthorById(int id)
        {
            return await _authorService.GetAuthorByIdAsync(id);
        }

        [HttpPost]
        [Route("authors")]
        public async Task<AuthorDto> CreateAuthor([FromBody] AuthorDto authorDto)
        {
            return await _authorService.CreateAuthorAsync(authorDto);
        }

        [HttpPut]
        [Route("authors/{id:int}")]
        public async Task<AuthorDto> UpdateAuthor(int id, [FromBody] AuthorDto authorDto)
        {
            authorDto.AuthorId = id;
            return await _authorService.UpdateAuthorAsync(authorDto);
        }

        [HttpDelete]
        [Route("authors/{id:int}")]
        public async Task DeleteAuthor(int id)
        {
            await _authorService.DeleteAuthorAsync(id);
        }
    }
}
