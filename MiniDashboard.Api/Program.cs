using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using MiniDashboard.Api.Services;
using MiniDashboard.Context.Interfaces;
using Scalar.AspNetCore;

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
            builder.Services.AddOpenApi(options =>
            {
                options.AddScalarTransformers();
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new OpenApiInfo
                    {
                        Title = "MiniDashboard API",
                        Version = "v1",
                        Description = "ASP.NET Core MVC + Scalar"
                    };

                    return Task.CompletedTask;
                });
            });
            builder.WebHost.UseUrls(builder.Configuration["Url"]!);

            builder.Services.AddDbContext<ApiDbContext>(options =>
            {
                options.UseSqlite(builder.Configuration["SqliteConnectionString"]);
            });
            builder.Services.AddTransient<IAuthorService, AuthorService>();
            builder.Services.AddTransient<IBooksService, BooksService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();

                app.MapScalarApiReference();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
