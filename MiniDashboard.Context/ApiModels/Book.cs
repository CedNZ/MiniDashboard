namespace MiniDashboard.Context.ApiModels
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; } = "";
        public string? SubTitle { get; set; }
        public string? Series { get; set; }
        public double? SeriesNumber { get; set; }
        public virtual ICollection<Author> Authors { get; set; } = [];
        public virtual ICollection<Genre> Genres { get; set; } = [];
    }
}
