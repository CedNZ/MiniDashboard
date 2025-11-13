namespace MiniDashboard.Context.ApiModels
{
    public class Genre
    {
        public int GenreId { get; set; }
        public string Name { get; set; } = "";
        public virtual ICollection<Book> Books { get; set; } = [];
    }
}
