using MiniDashboard.Context.ApiModels;

namespace MiniDashboard.Context.DTO
{
    public class GenreDto
    {
        public int GenreId { get; set; }
        public string Name { get; set; } = "";
        public List<BookDto> Books { get; set; } = [];

        public GenreDto()
        {
        }

        public GenreDto(Genre genre, bool noChildren = false)
        {
            GenreId = genre.GenreId;
            Name = genre.Name;
            Books = noChildren ? [] : genre.Books.Select(x => new BookDto(x, noChildren: true)).ToList();
        }

        public Genre ToEntity()
        {
            return new Genre
            {
                GenreId = GenreId,
                Name = Name,
                Books = Books.ConvertAll(x => x.ToEntity()),
            };
        }
    }
}
