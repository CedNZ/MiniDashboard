using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniDashboard.Api;
using MiniDashboard.Context.ApiModels;

namespace MiniDashboard.Tests.IntegrationTests.Infrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var contextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ApiDbContext));
            var contextOptionsDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApiDbContext>));
            var contextOptionsBuilderDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(Microsoft.EntityFrameworkCore.Infrastructure.IDbContextOptionsConfiguration<ApiDbContext>));
            if (contextDescriptor != null)
            {
                services.Remove(contextDescriptor);
            }
            if (contextOptionsDescriptor != null)
            {
                services.Remove(contextOptionsDescriptor);
            }
            if (contextOptionsBuilderDescriptor != null)
            {
                services.Remove(contextOptionsBuilderDescriptor);
            }

            var dbName = $"MiniDashboardIntegration_{Guid.NewGuid()}";

            services.AddDbContext<ApiDbContext>(options =>
            {
                options.UseInMemoryDatabase(dbName);
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
