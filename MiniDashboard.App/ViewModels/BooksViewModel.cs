using System.Collections.ObjectModel;
using System.Windows;
using MiniDashboard.App.Commands;
using MiniDashboard.Context.DTO;
using MiniDashboard.Context.Interfaces;

namespace MiniDashboard.App.ViewModels
{
    public class BooksViewModel : ViewModelBase
    {
        private readonly IBooksService _booksService;

        public ObservableCollection<BookDto> Books { get; set; } = [];

        public AsyncRelayCommand LoadBooksCommand { get; }
        public AsyncRelayCommand SaveBookCommand { get; }
        public RelayCommand OpenCreateDialogCommand { get; }
        public RelayCommand<BookDto> OpenEditDialogCommand { get; }
        public RelayCommand CancelDialogCommand { get; }
        public AsyncRelayCommand<BookDto> DeleteBookCommand { get; }

        public BooksViewModel(IBooksService booksService)
        {
            _booksService = booksService;

            LoadBooksCommand = new AsyncRelayCommand(LoadBooksAsync);
            SaveBookCommand = new AsyncRelayCommand(SaveAsync, CanSave);
            OpenCreateDialogCommand = new RelayCommand(OpenCreateDialog);
            OpenEditDialogCommand = new RelayCommand<BookDto>(OpenEditDialog);
            CancelDialogCommand = new RelayCommand(CancelDialog);
            DeleteBookCommand = new AsyncRelayCommand<BookDto>(DeleteBookAsync);

            _ = LoadBooksAsync();
        }

        #region CRUD Properties
        public bool IsDialogOpen
        {
            get;
            set => SetField(ref field, value);
        }

        public int? BookId
        {
            get;
            set => SetField(ref field, value);
        }

        public string? Title
        {
            get;
            set => SetField(ref field, value, commands: [SaveBookCommand]);
        }

        public string? SubTitle
        {
            get;
            set => SetField(ref field, value);
        }

        public string? Series
        {
            get;
            set => SetField(ref field, value);
        }

        public string? SeriesNumber
        {
            get;
            set => SetField(ref field, value);
        }

        public string? Authors
        {
            get;
            set => SetField(ref field, value, commands: [SaveBookCommand]);
        }

        public string? Genres
        {
            get;
            set => SetField(ref field, value);
        }

        public string? Query
        {
            get;
            set => SetField(ref field, value);
        }

        public string? ModalHeader
        {
            get;
            set => SetField(ref field, value);
        }

        public bool Loading
        {
            get;
            set => SetField(ref field, value);
        }

        protected bool SetField<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null!, params List<CommandBase> commands)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);

            commands.ForEach(x => x.RaiseCanExecuteChanged());

            return true;
        }
        #endregion

        private async Task LoadBooksAsync()
        {
            Loading = true;

            Books.Clear();
            var books = await _booksService.GetAllAsync();
            foreach (var bookDto in books)
            {
                Books.Add(bookDto);
            }

            Loading = false;
        }

        private void OpenCreateDialog()
        {
            BookId = null;
            Title = "";
            SubTitle = null;
            Series = null;
            SeriesNumber = null;
            Authors = null;
            Genres = null;
            ModalHeader = "Create Book";
            IsDialogOpen = true;
        }

        private void OpenEditDialog(BookDto book)
        {
            BookId = book.BookId;
            Title = book.Title;
            SubTitle = book.SubTitle;
            Series = book.Series;
            SeriesNumber = book.SeriesNumber.ToString();
            Authors = string.Join(';', book.Authors.Select(x => x.Name).Order());
            Genres = string.Join(';', book.Genres.Select(x => x.Name).Order());
            ModalHeader = $"Edit Book ({BookId})";
            IsDialogOpen = true;
        }

        private void CancelDialog()
        {
            IsDialogOpen = false;
        }

        private async Task DeleteBookAsync(BookDto book)
        {
            var messageBoxResult = MessageBox.Show($"Are you sure you want to delete '{book.Title}'?",
                "Delete Confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Loading = true;
                await _booksService.DeleteBookAsync(book.BookId);
                Books.RemoveAt(Books.IndexOf(book));
                Loading = false;
            }
        }

        private bool CanSave() => !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Authors);

        private async Task SaveAsync()
        {
            Loading = true;
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

            if (book.BookId > 0)
            {
                var updated = await _booksService.UpdateBookAsync(book);
                if (updated != null)
                {
                    var inMemBook = Books.FirstOrDefault(x => x.BookId == book.BookId);
                    if (inMemBook != null)
                    {
                        var index = Books.IndexOf(inMemBook);
                        Books[index] = updated;
                    }
                    else
                    {
                        Books.Add(updated);
                    }
                }
            }
            else
            {
                var created = await _booksService.CreateBookAsync(book);
                if (created != null)
                {
                    Books.Add(created);
                }
            }

            IsDialogOpen = false;
            Loading = false;
        }
    }
}
