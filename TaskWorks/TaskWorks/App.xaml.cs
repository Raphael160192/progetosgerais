using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TaskWorks.ViewModels;
using TaskWorks.Business.Services.Implementations;
using TaskWorks.Business.Services.Interfaces;
using TaskWorks.Data;
using TaskWorks.Data.DataContext;

namespace TaskWorks
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainViewModel>();

            // Register data context
            services.AddScoped<TaskWorksContext>();

            // Register services
            services.AddScoped<IEventoService, EventoService>();
            services.AddScoped<IMovimentacaoService, MovimentacaoService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<IDadosProdutividadeService, DadosProdutividadeService>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            mainWindow.DataContext = _serviceProvider.GetService<MainViewModel>();
            mainWindow.Show();
        }
    }
}
