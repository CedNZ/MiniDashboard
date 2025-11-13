using System.Collections.ObjectModel;
using System.Windows.Input;
using MiniDashboard.App.Commands;
using MiniDashboard.Context.DTO;
using MiniDashboard.Context.Interfaces;

namespace MiniDashboard.App.ViewModels
{
    public class AuthorsViewModel : ViewModelBase
    {
        private readonly IAuthorService _authorService;

        public ObservableCollection<AuthorDto> Authors { get; set; } = [];

        public ICommand LoadAuthorsCommand { get; }

        public AuthorsViewModel(IAuthorService authorService)
        {
            _authorService = authorService;

            LoadAuthorsCommand = new AsyncRelayCommand(LoadAuthorsAsync);
        }

        private async Task LoadAuthorsAsync()
        {
            Authors.Clear();
            var authors = await _authorService.GetAuthorsAsync();
            foreach (var authorDto in authors)
            {
                Authors.Add(authorDto);
            }
        }
    }
}
