using System.Collections.ObjectModel;
using MiniDashboard.Context.DTO;
using MiniDashboard.Context.Interfaces;

namespace MiniDashboard.App.ViewModels
{
    public class BooksViewModel : ViewModelBase
    {
        private readonly IBooksService _booksService;

        public ObservableCollection<BookDto> Books { get; set; } = [];

        public AsyncRelayCommand LoadBooksCommand { get; }
        public AsyncRelayCommand CreateBookCommand { get; }
        public RelayCommand OpenCreateDialogCommand { get; }
        public RelayCommand CancelDialogCommand { get; }

        public BooksViewModel(IBooksService booksService)
        {
            _booksService = booksService;

            LoadBooksCommand = new AsyncRelayCommand(LoadBooksAsync);
            CreateBookCommand = new AsyncRelayCommand(SaveAsync, CanSave);
            OpenCreateDialogCommand = new RelayCommand(OpenCreateDialog);
            CancelDialogCommand = new RelayCommand(CancelDialog);
        }

        #region CRUD Properties
        public bool IsDialogOpen
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

        public int? BookId
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

        public string? Title
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

        public string? SubTitle
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

        public string? Series
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

        public string? SeriesNumber
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

        public string? Authors
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

        public string? Genres
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
        #endregion

        private async Task LoadBooksAsync()
        {
            Books.Clear();
            var books = await _booksService.GetAllAsync();
            foreach (var bookDto in books)
            {
                Books.Add(bookDto);
            }
        }

        private void OpenCreateDialog()
        {
            Title = "";
            SubTitle = null;
            Series = null;
            SeriesNumber = null;
            Authors = null;
            Genres = null;
            IsDialogOpen = true;
        }

        private void CancelDialog()
        {
            IsDialogOpen = false;
        }

        private bool CanSave() => Title != "" && Authors != "";

        private async Task SaveAsync()
        {
            var seriesNum = double.TryParse(SeriesNumber, out var sNum) ? sNum : (double?)null;

            var book = new BookDto
            {
                BookId = BookId ?? 0,
                Title = Title ?? "",
                SubTitle = SubTitle,
                Series = Series,
                SeriesNumber = seriesNum,
                Authors = Authors?.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(x => new AuthorDto
                {
                    Name = x,
                }).ToList() ?? [],
                Genres = Genres?.Split(';', StringSplitOptions.RemoveEmptyEntries).Select(x => new GenreDto()
                {
                    Name = x,
                }).ToList() ?? [],
            };

            var created = await _booksService.CreateBookAsync(book);
            if (created != null)
            {
                Books.Add(created);
            }

            IsDialogOpen = false;
        }
    }
}
