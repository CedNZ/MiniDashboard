using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MiniDashboard.App.Services;
using MiniDashboard.App.ViewModels;
using MiniDashboard.Context.Interfaces;

namespace MiniDashboard.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static IServiceProvider Services { get; set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);

            Services = services.BuildServiceProvider();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("./appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddHttpClient("API", c =>
            {
                c.BaseAddress = new Uri(config["Url"]!);
            });
            services.AddScoped<IAuthorService, AppAuthorsService>();
            services.AddScoped<IBooksService, AppBooksService>();

            services.AddSingleton<AuthorsViewModel>();
            services.AddSingleton<BooksViewModel>();
            services.AddSingleton<MainViewModel>();

            services.AddSingleton<MainWindow>();
        }
    }

}
