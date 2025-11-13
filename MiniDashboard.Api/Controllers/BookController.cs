using Microsoft.AspNetCore.Mvc;
using MiniDashboard.Api.Services.Interfaces;
using MiniDashboard.Context.DTO;

namespace MiniDashboard.Api.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBooksService _booksService;

        public BookController(IBooksService booksService)
        {
            _booksService = booksService;
        }

        [HttpGet]
        [Route("books")]
        public async Task<List<BookDto>> GetBooks()
        {
            var books = await _booksService.GetAllAsync();
            return books.ConvertAll(x => new BookDto(x));
        }

        [HttpGet]
        [Route("books/{id:int}")]
        public async Task<BookDto?> GetBookById(int id)
        {
            var book = await _booksService.GetByIdAsync(id);
            if (book != null)
            {
                return new BookDto(book);
            }

            return null;
        }

        [HttpGet]
        [Route("books/search")]
        public async Task<List<BookDto>> SearchBooks([FromQuery] string query)
        {
            var books = await _booksService.QueryByNameAsync(query);
            return books.ConvertAll(x => new BookDto(x));
        }

        [HttpPost]
        [Route("books")]
        public async Task<BookDto> CreateBook([FromBody] BookDto bookDto)
        {
            var book = await _booksService.CreateBookAsync(bookDto.ToEntity());
            return new BookDto(book);
        }

        [HttpPut]
        [Route("books/{id:int}")]
        public async Task<BookDto> UpdateBook(int id, [FromBody] BookDto bookDto)
        {
            bookDto.BookId = id;
            var book = await _booksService.UpdateBookAsync(bookDto.ToEntity());
            return new BookDto(book);
        }

        [HttpDelete]
        [Route("books/{id:int}")]
        public async Task DeleteBook(int id)
        {
            await _booksService.DeleteBookAsync(id);
        }
    }
}
