using System.IO;
using System.IO.Compression;
using System.Windows;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using WpfWithAspNetCore.ViewModels;

namespace WpfWithAspNetCore;

public partial class App : Application
{
    public IServiceProvider ServiceProvider { get; }
    private readonly IServiceScope? _serviceScope;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly WebApplication _host;

    public App()
    {
        DirectoryInfo staticFilesDirectory = new("Resources/wwwroot");
     
        // clean previous static files
        if (staticFilesDirectory.Exists)
        {
            staticFilesDirectory.Delete(true);
        }

        // extract static files
        ZipFile.ExtractToDirectory($"{staticFilesDirectory}.zip", $"{staticFilesDirectory}");


        _cancellationTokenSource = new CancellationTokenSource();

        WebApplicationOptions options = new()
        {
#if ASPNET_DEVELOPMENT
            EnvironmentName = "Development"
#endif
        };

        var builder = WebApplication.CreateBuilder(options);

        builder.Services.AddSingleton<MainWindow>();
        builder.Services.AddSingleton<MainWindowViewModel>();
        builder.Services.AddControllers();
        
        _host = builder.Build();

        _host.UseHttpsRedirection();

        _host.UseFileServer(new FileServerOptions
            { FileProvider = new PhysicalFileProvider(staticFilesDirectory.FullName) });

        _host.MapGet("/", () => $"{DateTime.Now:O}");
        _host.MapControllers();

        _serviceScope = _host.Services.CreateScope();
        ServiceProvider = _serviceScope.ServiceProvider;
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync(_cancellationTokenSource.Token);

        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        _serviceScope?.Dispose();
        await _host.StopAsync(_cancellationTokenSource.Token);
        await _host.DisposeAsync();
        base.OnExit(e);
    }
}
