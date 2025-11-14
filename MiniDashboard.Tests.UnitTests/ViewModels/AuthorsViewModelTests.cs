using MiniDashboard.App.ViewModels;
using MiniDashboard.Context.DTO;
using MiniDashboard.Context.Interfaces;
using MiniDashboard.Tests.UnitTests.TestHelpers;
using NSubstitute;

namespace MiniDashboard.Tests.UnitTests.ViewModels;

public class AuthorsViewModelTests
{
    [Fact]
    public async Task LoadAuthorsAsync_PopulatesAuthors()
    {
        var authors = new List<AuthorDto>
        {
            new() { AuthorId = 1, Name = "Ada" },
            new() { AuthorId = 2, Name = "Bob" },
        };

        var serviceMock = Substitute.For<IAuthorService>();
        serviceMock.GetAuthorsAsync().Returns(Task.FromResult(authors));

        var viewModel = new AuthorsViewModel(serviceMock);

        await AsyncTestHelper.WaitForConditionAsync(() => viewModel.Authors.Count == authors.Count, TimeSpan.FromSeconds(1));

        Assert.False(viewModel.Loading);
        Assert.Equal(new[] { "Ada", "Bob" }, viewModel.Authors.Select(a => a.Name));
    }

    [Fact]
    public async Task LoadAuthorsAsync_ResetsLoadingWhenFailureOccurs()
    {
        var serviceMock = Substitute.For<IAuthorService>();
        serviceMock.GetAuthorsAsync().Returns(
            Task.FromResult(new List<AuthorDto>()),
            Task.FromException<List<AuthorDto>>(new InvalidOperationException("boom")));

        var viewModel = new AuthorsViewModel(serviceMock);
        await AsyncTestHelper.WaitForConditionAsync(() => !viewModel.Loading, TimeSpan.FromSeconds(1));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => AsyncTestHelper.InvokePrivateAsync(viewModel, "LoadAuthorsAsync"));
        Assert.Equal("boom", exception.Message);
        Assert.False(viewModel.Loading);
    }
}
