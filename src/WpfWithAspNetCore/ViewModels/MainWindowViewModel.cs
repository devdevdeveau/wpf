using System.Windows.Input;
using Microsoft.Extensions.Configuration;

namespace WpfWithAspNetCore.ViewModels;

public class MainWindowViewModel
{
    public string? Url { get; init; }

    public ICommand OpenBrowser { get; init; }

    public MainWindowViewModel(IConfiguration configuration)
    {
        Url = configuration["Kestrel:Endpoints:Https:Url"] ?? "Missing Uri";

        OpenBrowser = new OpenBrowserCommand(Url);
    }

    private class OpenBrowserCommand : ICommand
    {
        private readonly string _url;

        public OpenBrowserCommand(string url)
        {
            _url = url;
        }

        public bool CanExecute(object? parameter)
        {
            return Uri.IsWellFormedUriString(_url, UriKind.Absolute);
        }

        public void Execute(object? parameter)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(_url)
            {
                UseShellExecute = true
            });
        }

        public event EventHandler? CanExecuteChanged;
    }
}