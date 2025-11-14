using MiniDashboard.App.ViewModels;
using MiniDashboard.Context.DTO;
using MiniDashboard.Context.Interfaces;
using MiniDashboard.Tests.UnitTests.TestHelpers;
using NSubstitute;

namespace MiniDashboard.Tests.UnitTests.ViewModels;

public class BooksViewModelTests
{
    [Fact]
    public async Task LoadBooksAsync_PopulatesBooksAndResetsLoading()
    {
        // Arrange
        var books = new List<BookDto>
        {
            new()
            {
                BookId = 1,
                Title = "The Testing Handbook",
                Authors = [new AuthorDto { Name = "Ada" }],
                Genres = [new GenreDto { Name = "Education" }],
            },
        };

        var serviceMock = CreateBooksServiceMock();
        serviceMock.GetAllAsync().Returns(Task.FromResult(books));

        var viewModel = new BooksViewModel(serviceMock);
        await AsyncTestHelper.WaitForConditionAsync(() => viewModel.Books.Count == books.Count, TimeSpan.FromSeconds(1));

        // Assert
        Assert.False(viewModel.Loading);
        Assert.Collection(viewModel.Books, book =>
        {
            Assert.Equal("The Testing Handbook", book.Title);
            Assert.Equal("Ada", book.Authors.Single().Name);
        });
    }

    [Fact]
    public async Task SearchBooksAsync_UsesQueryAndUpdatesBooks()
    {
        // Arrange
        var serviceMock = CreateBooksServiceMock();
        var searchResults = new List<BookDto>
        {
            new() { BookId = 99, Title = "Mystery of Tests", Authors = [new AuthorDto { Name = "Casey" }] },
        };

        serviceMock.QueryByNameAsync("Mystery").Returns(Task.FromResult(searchResults));

        var viewModel = new BooksViewModel(serviceMock);
        await AsyncTestHelper.WaitForConditionAsync(() => !viewModel.Loading, TimeSpan.FromSeconds(1));

        viewModel.Query = "Mystery";

        // Act
        await AsyncTestHelper.InvokePrivateAsync(viewModel, "SearchBooksAsync");

        // Assert
        Assert.Collection(viewModel.Books, book => Assert.Equal("Mystery of Tests", book.Title));
    }

    [Fact]
    public void SearchBooksCommand_ReflectsQueryState()
    {
        var serviceMock = CreateBooksServiceMock();
        var viewModel = new BooksViewModel(serviceMock);

        Assert.False(viewModel.SearchBooksCommand.CanExecute(null));

        viewModel.Query = "Ada";

        Assert.True(viewModel.SearchBooksCommand.CanExecute(null));
    }

    [Fact]
    public void OpenCreateDialog_ResetsFormState()
    {
        var serviceMock = CreateBooksServiceMock();
        var viewModel = new BooksViewModel(serviceMock)
        {
            BookId = 10,
            Title = "Existing",
            SubTitle = "Old",
            Series = "Legacy",
            SeriesNumber = "2",
            Authors = "Ada;Bob",
            Genres = "Tech",
        };

        viewModel.OpenCreateDialogCommand.Execute(null);

        Assert.True(viewModel.IsDialogOpen);
        Assert.Null(viewModel.BookId);
        Assert.Equal(string.Empty, viewModel.Title);
        Assert.Null(viewModel.SubTitle);
        Assert.Null(viewModel.Series);
        Assert.Null(viewModel.SeriesNumber);
        Assert.Null(viewModel.Authors);
        Assert.Null(viewModel.Genres);
        Assert.Equal("Create Book", viewModel.ModalHeader);
    }

    [Fact]
    public async Task SaveAsync_CreatesBookAndAddsToCollection()
    {
        // Arrange
        var createdBook = new BookDto
        {
            BookId = 42,
            Title = "New Adventures",
            Authors = [new AuthorDto { Name = "Ada" }, new AuthorDto { Name = "Bob" }],
            Genres = [new GenreDto { Name = "Sci-Fi" }],
        };

        BookDto? captured = null;

        var serviceMock = CreateBooksServiceMock();
        serviceMock.CreateBookAsync(Arg.Any<BookDto>())
            .Returns(call =>
            {
                captured = call.Arg<BookDto>();
                return Task.FromResult<BookDto?>(createdBook);
            });

        var viewModel = new BooksViewModel(serviceMock)
        {
            Title = "New Adventures",
            Authors = "Ada;Bob",
            Genres = "Sci-Fi",
            SeriesNumber = "",
            IsDialogOpen = true,
        };

        await AsyncTestHelper.WaitForConditionAsync(() => !viewModel.Loading, TimeSpan.FromSeconds(1));

        // Act
        await AsyncTestHelper.InvokePrivateAsync(viewModel, "SaveAsync");

        // Assert
        Assert.NotNull(captured);
        Assert.Equal("New Adventures", captured.Title);
        Assert.Equal(new[] { "Ada", "Bob" }, captured.Authors.Select(a => a.Name));
        Assert.Single(viewModel.Books, b => b.BookId == 42);
        Assert.False(viewModel.IsDialogOpen);
        Assert.False(viewModel.Loading);
    }

    [Fact]
    public async Task SaveAsync_UpdatesExistingBookInCollection()
    {
        // Arrange
        var existing = new BookDto
        {
            BookId = 7,
            Title = "Old Title",
            Authors = [new AuthorDto { Name = "Ada" }],
        };

        var serviceMock = CreateBooksServiceMock();
        serviceMock.GetAllAsync().Returns(Task.FromResult(new List<BookDto> { existing }));

        var updated = new BookDto
        {
            BookId = 7,
            Title = "Refined Title",
            Authors = [new AuthorDto { Name = "Ada" }],
        };

        BookDto? captured = null;
        serviceMock.UpdateBookAsync(Arg.Any<BookDto>())
            .Returns(call =>
            {
                captured = call.Arg<BookDto>();
                return Task.FromResult<BookDto?>(updated);
            });

        var viewModel = new BooksViewModel(serviceMock)
        {
            BookId = 7,
            Title = "Refined Title",
            Authors = "Ada",
            IsDialogOpen = true,
        };

        await AsyncTestHelper.WaitForConditionAsync(() => viewModel.Books.Count == 1, TimeSpan.FromSeconds(1));

        // Act
        await AsyncTestHelper.InvokePrivateAsync(viewModel, "SaveAsync");

        // Assert
        Assert.NotNull(captured);
        Assert.Equal(7, captured.BookId);
        Assert.Equal("Refined Title", viewModel.Books.Single().Title);
        Assert.False(viewModel.IsDialogOpen);
    }

    private static IBooksService CreateBooksServiceMock()
    {
        var serviceMock = Substitute.For<IBooksService>();
        serviceMock.GetAllAsync().Returns(Task.FromResult(new List<BookDto>()));
        serviceMock.QueryByNameAsync(Arg.Any<string>()).Returns(Task.FromResult(new List<BookDto>()));
        serviceMock.CreateBookAsync(Arg.Any<BookDto>()).Returns(Task.FromResult<BookDto?>(null));
        serviceMock.UpdateBookAsync(Arg.Any<BookDto>()).Returns(Task.FromResult<BookDto?>(null));
        serviceMock.DeleteBookAsync(Arg.Any<int>()).Returns(Task.CompletedTask);
        serviceMock.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult<BookDto?>(null));
        return serviceMock;
    }
}
