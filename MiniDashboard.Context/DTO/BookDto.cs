using MiniDashboard.Context.ApiModels;

namespace MiniDashboard.Context.DTO
{
    public class BookDto
    {
        public int BookId { get; set; }
        public string Title { get; set; } = "";
        public string? SubTitle { get; set; }
        public string? Series { get; set; }
        public double? SeriesNumber { get; set; }
        public List<AuthorDto> Authors { get; set; } = [];
        public List<GenreDto> Genres { get; set; } = [];

        public string AuthorString => string.Join("; ", Authors.ConvertAll(x => x.Name).Order());
        public string GenreString => string.Join("; ", Genres.ConvertAll(x => x.Name).Order());

        public BookDto()
        {
        }

        public BookDto(Book book, bool noChildren = false)
        {
            BookId = book.BookId;
            Title = book.Title;
            SubTitle = book.SubTitle;
            Series = book.Series;
            SeriesNumber = book.SeriesNumber;
            Authors = noChildren ? [] : book.Authors.Select(x => new AuthorDto(x, noChildren: true)).ToList();
            Genres = noChildren ? [] : book.Genres.Select(x => new GenreDto(x, noChildren: true)).ToList();
        }

        public Book ToEntity()
        {
            return new Book
            {
                BookId = BookId,
                Title = Title,
                SubTitle = SubTitle,
                Series = Series,
                SeriesNumber = SeriesNumber,
                Authors = Authors.ConvertAll(x => x.ToEntity()),
                Genres = Genres.ConvertAll(x => x.ToEntity()),
            };
        }
    }
}
