using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MiniDashboard.Context.ApiModels;

namespace MiniDashboard.Api
{
    public class ApiDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Genre> Genres { get; set; }

        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>(e =>
            {
                e.HasKey(x => x.AuthorId);

                e.Property(x => x.Name)
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Book>(e =>
            {
                e.HasKey(x => x.BookId);

                e.Property(x => x.Title)
                    .HasMaxLength(100);

                e.Property(x => x.SubTitle)
                    .HasMaxLength(200);

                e.Property(x => x.Series)
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Genre>(e =>
            {
                e.HasKey(x => x.GenreId);

                e.Property(x => x.Name)
                    .HasMaxLength(50);
            });

            base.OnModelCreating(modelBuilder);
        }
    }

    public class ApiDbContextFactory : IDesignTimeDbContextFactory<ApiDbContext>
    {
        public ApiDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApiDbContext>();
            optionsBuilder.UseSqlite("Data Source=/tmp/minidashboard.sqlite");
            return new ApiDbContext(optionsBuilder.Options);
        }
    }
}
