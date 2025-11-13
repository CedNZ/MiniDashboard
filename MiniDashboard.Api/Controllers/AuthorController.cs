using Microsoft.AspNetCore.Mvc;
using MiniDashboard.Api.Services.Interfaces;
using MiniDashboard.Context.DTO;

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
            var authors = await _authorService.GetAuthorsAsync();
            return authors.ConvertAll(x => new AuthorDto(x));
        }

        [HttpGet]
        [Route("authors/{id:int}")]
        public async Task<AuthorDto?> GetAuthorById(int id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author is not null)
            {
                return new AuthorDto(author);
            }

            return null;
        }

        [HttpPost]
        [Route("authors")]
        public async Task<AuthorDto> CreateAuthor([FromBody] AuthorDto authorDto)
        {
            var author = await _authorService.CreateAuthorAsync(authorDto.ToEntity());
            return new AuthorDto(author);
        }

        [HttpPut]
        [Route("authors/{id:int}")]
        public async Task<AuthorDto> UpdateAuthor(int id, [FromBody] AuthorDto authorDto)
        {
            authorDto.AuthorId = id;
            var author = await _authorService.UpdateAuthorAsync(authorDto.ToEntity());
            return new AuthorDto(author);
        }

        [HttpDelete]
        [Route("authors/{id:int}")]
        public async Task DeleteAuthor(int id)
        {
            await _authorService.DeleteAuthorAsync(id);
        }
    }
}
