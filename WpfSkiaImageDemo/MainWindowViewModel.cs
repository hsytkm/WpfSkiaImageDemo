using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;

namespace WpfSkiaImageDemo;

public sealed partial class MainWindowViewModel : ObservableObject
{
    private const string ImagePath = @"Assets/image1.jpg";

    [ObservableProperty]
    private double _counter;

    [ObservableProperty]
    private BitmapSource? _wpfImage;

    [ObservableProperty]
    private SKBitmap? _skiaImage;

    [ObservableProperty]
    private string? _message;

    public MainWindowViewModel()
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
        WpfImage = null;

        (SkiaImage as IDisposable)?.Dispose();
        SkiaImage = null;

        Message = null;

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    [RelayCommand]
    private async Task LoadWpfImageAsync()
    {
        ClearImage();
        var sw = Stopwatch.StartNew();

        var image = await Task.Run(() => BitmapSourceEx.ToBitmapSource(ImagePath));
        WpfImage = image;

        sw.Stop();
        Message = $"Wpf : {sw.ElapsedMilliseconds} msec";
    }

    [RelayCommand]
    private async Task LoadSkiaImageAsync()
    {
        ClearImage();
        var sw = Stopwatch.StartNew();

#if false
        var bs = await File.ReadAllBytesAsync(ImagePath);
        var image = await Task.Run(() => SKBitmap.Decode(bs.AsSpan()));
#else
        // 上と同等のパフォーマンスやったけど、こちらの方が伸び代を感じるので…
        using var stream = File.OpenRead(ImagePath);
        var image = await Task.Run(() => SKBitmap.Decode(stream));
#endif
        SkiaImage = image;

        sw.Stop();
        Message = $"Skia: {sw.ElapsedMilliseconds} msec";
    }
}
