namespace MiniDashboard.App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public AuthorsViewModel Authors { get; }
        public BooksViewModel Books { get; }

        public MainViewModel(AuthorsViewModel authors, BooksViewModel books)
        {
            Authors = authors;
            Books = books;
            SelectedTab = 0;
        }

        public int SelectedTab
        {
            get;
            set
            {
                if (field != value)
                {
                    field = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    public enum TabViews
    {
        Books,
        Authors,
    }
}
