using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;
using SkiaSharp.Views.WPF;

namespace WpfSkiaImageDemo;

public sealed partial class MainWindowViewModel : ObservableObject
{
    private const string ImagePath = @"Assets/image1.jpg";

    [ObservableProperty]
    private double _counter;

    [ObservableProperty]
    private BitmapSource? _wpfImage1;

    [ObservableProperty]
    private BitmapSource? _wpfImage2;

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
        WpfImage1 = null;
        WpfImage2 = null;

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

        WpfImage1 = await Task.Run(() => BitmapSourceEx.ToBitmapSource(ImagePath));

        sw.Stop();
        Message = $"Wpf : {sw.ElapsedMilliseconds} msec";
    }

    [RelayCommand]
    private async Task LoadWpfImageFromSkiaAsync()
    {
        ClearImage();
        var sw = Stopwatch.StartNew();

        using var image = await LoadSKBitmapAsync(ImagePath);
        WpfImage2 = image.ToWriteableBitmap();

        sw.Stop();
        Message = $"SkiaSharp : {sw.ElapsedMilliseconds} msec";
    }

    [RelayCommand]
    private async Task LoadSkiaImageAsync()
    {
        ClearImage();
        var sw = Stopwatch.StartNew();

        SkiaImage = await LoadSKBitmapAsync(ImagePath);

        sw.Stop();
        Message = $"SkiaImageView : {sw.ElapsedMilliseconds} msec";
    }

    private static async Task<SKBitmap> LoadSKBitmapAsync(string path)
    {
#if false
        var bs = await File.ReadAllBytesAsync(path);
        return await Task.Run(() => SKBitmap.Decode(bs.AsSpan()));
#else
        // 上と同等のパフォーマンスやったけど、こちらの方が伸び代を感じるので…
        using var stream = File.OpenRead(path);
        return await Task.Run(() => SKBitmap.Decode(stream));
#endif
    }
}
