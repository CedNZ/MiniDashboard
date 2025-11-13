using MiniDashboard.Context.ApiModels;

namespace MiniDashboard.Context.DTO
{
    public class AuthorDto
    {
        public int AuthorId { get; set; }
        public string Name { get; set; } = "";
        public List<BookDto> Books { get; set; } = [];

        public int BookCount => Books.Count;

        public AuthorDto()
        {
        }

        public AuthorDto(Author author, bool noChildren = false)
        {
            AuthorId = author.AuthorId;
            Name = author.Name;
            Books = noChildren ? [] : author.Books.Select(x => new BookDto(x, noChildren: true)).ToList();
        }

        public Author ToEntity()
        {
            return new Author
            {
                AuthorId = AuthorId,
                Name = Name,
                Books = Books.ConvertAll(x => x.ToEntity()),
            };
        }
    }
}
