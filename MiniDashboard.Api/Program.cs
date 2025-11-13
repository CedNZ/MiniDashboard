
using Microsoft.EntityFrameworkCore;
using MiniDashboard.Api.Services;
using MiniDashboard.Context.Interfaces;

namespace MiniDashboard.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddDbContext<ApiDbContext>(options =>
            {
                options.UseSqlite("Data Source=/tmp/minidashboard.sqlite");
            });
            builder.Services.AddScoped<IAuthorService, AuthorService>();
            builder.Services.AddScoped<IBooksService, BooksService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
