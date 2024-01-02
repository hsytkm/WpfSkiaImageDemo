using System.Diagnostics;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaImageDemo.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{
    private const string ImagePath = @"Assets/image1.jpg";

    [ObservableProperty]
    private double _counter;

    [ObservableProperty]
    private Bitmap? _bitmapImage;

    [ObservableProperty]
    private string? _message;

    public MainViewModel()
    {
        Task.Run(async () =>
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(10));
            while (await timer.WaitForNextTickAsync())
            {
                Counter += 0.01;
            }
        });
    }

    [RelayCommand]
    private void ClearImage()
    {
        (BitmapImage as IDisposable)?.Dispose();
        BitmapImage = null;

        Message = null;

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    [RelayCommand]
    private async Task LoadImageAsync()
    {
        ClearImage();
        var sw = Stopwatch.StartNew();

        var image = await Task.Run(() => new Bitmap(ImagePath));
        BitmapImage = image;

        sw.Stop();
        Message = $"{sw.ElapsedMilliseconds} msec";
    }
}
