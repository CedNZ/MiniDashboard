namespace MiniDashboard.App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public AuthorsViewModel Authors { get; }
        public BooksViewModel Books { get; }

        public event Action<int>? TabChanged;

        public MainViewModel(AuthorsViewModel authors, BooksViewModel books)
        {
            Authors = authors;
            Books = books;

            TabChanged += tabIndex =>
            {
                if (tabIndex == 1)
                {
                    Authors.LoadAuthorsCommand.Execute(null);
                }
            };

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

                    TabChanged?.Invoke(value);
                }
            }
        }
    }
}
