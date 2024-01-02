using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Windows.Graphics;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Search;

namespace WinUI3ImageDemo;

public sealed partial class MainWindow : Window
{
    public MainWindowViewModel ViewModel { get; } = new();

    public MainWindow()
    {
        this.InitializeComponent();

        AppWindow.Resize(new SizeInt32(500, 300));
    }
}