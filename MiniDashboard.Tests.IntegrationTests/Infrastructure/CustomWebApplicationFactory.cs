using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniDashboard.Api;
using MiniDashboard.Context.ApiModels;
using System.Linq;

namespace MiniDashboard.Tests.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApiDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApiDbContext>(options =>
            {
                options.UseInMemoryDatabase($"MiniDashboardIntegration_{Guid.NewGuid()}");
            });

            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            SeedData.Populate(context);
        });
    }
}

public static class SeedData
{
    public static void Populate(ApiDbContext context)
    {
        context.Authors.RemoveRange(context.Authors);
        context.Genres.RemoveRange(context.Genres);
        context.Books.RemoveRange(context.Books);
        context.SaveChanges();

        var author = new Author { Name = "Ada Lovelace" };
        var genre = new Genre { Name = "Technology" };
        var book = new Book
        {
            Title = "Analytical Engines",
            SubTitle = "Computing Origins",
            Series = "History",
            SeriesNumber = 1,
            Authors = { author },
            Genres = { genre },
        };
        author.Books.Add(book);
        genre.Books.Add(book);

        context.Authors.Add(author);
        context.Genres.Add(genre);
        context.Books.Add(book);
        context.SaveChanges();
    }
}
