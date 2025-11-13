using Microsoft.AspNetCore.Mvc;
using MiniDashboard.Context.DTO;
using MiniDashboard.Context.Interfaces;

namespace MiniDashboard.Api.Controllers
{
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBooksService _booksService;

        public BookController(IBooksService booksService)
        {
            _booksService = booksService;
        }

        [HttpGet]
        [Route("books")]
        [ProducesResponseType<List<BookDto>>(StatusCodes.Status200OK)]
        public async Task<List<BookDto>> GetBooks()
        {
            return await _booksService.GetAllAsync();
        }

        [HttpGet]
        [Route("books/{id:int}")]
        [ProducesResponseType<BookDto>(StatusCodes.Status200OK)]
        public async Task<BookDto?> GetBookById(int id)
        {
            return await _booksService.GetByIdAsync(id);
        }

        [HttpGet]
        [Route("books/search")]
        [ProducesResponseType<List<BookDto>>(StatusCodes.Status200OK)]
        public async Task<List<BookDto>> SearchBooks([FromQuery] string query)
        {
            return await _booksService.QueryByNameAsync(query);
        }

        [HttpPost]
        [Route("books")]
        [ProducesResponseType<BookDto>(StatusCodes.Status200OK)]
        public async Task<BookDto?> CreateBook([FromBody] BookDto bookDto)
        {
            return await _booksService.CreateBookAsync(bookDto);
        }

        [HttpPut]
        [Route("books/{id:int}")]
        [ProducesResponseType<BookDto>(StatusCodes.Status200OK)]
        public async Task<BookDto?> UpdateBook(int id, [FromBody] BookDto bookDto)
        {
            bookDto.BookId = id;
            return await _booksService.UpdateBookAsync(bookDto);
        }

        [HttpDelete]
        [Route("books/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task DeleteBook(int id)
        {
            await _booksService.DeleteBookAsync(id);
        }
    }
}
