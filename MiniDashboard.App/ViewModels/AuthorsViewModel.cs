using System.Collections.ObjectModel;
using MiniDashboard.App.Commands;
using MiniDashboard.Context.DTO;
using MiniDashboard.Context.Interfaces;

namespace MiniDashboard.App.ViewModels
{
    public class AuthorsViewModel : ViewModelBase
    {
        private readonly IAuthorService _authorService;

        public ObservableCollection<AuthorDto> Authors { get; set; } = [];

        public AsyncRelayCommand LoadAuthorsCommand { get; }

        public AuthorsViewModel(IAuthorService authorService)
        {
            _authorService = authorService;

            LoadAuthorsCommand = new AsyncRelayCommand(LoadAuthorsAsync);

            _ = LoadAuthorsAsync();
        }

        public bool Loading
        {
            get;
            set => SetField(ref field, value);
        }

        private async Task LoadAuthorsAsync()
        {
            Loading = true;
            try
            {
                Authors.Clear();
                var authors = await _authorService.GetAuthorsAsync();
                foreach (var authorDto in authors)
                {
                    Authors.Add(authorDto);
                }
            }
            finally
            {
                Loading = false;
            }
        }
    }
}
