using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;

namespace WinUI3ImageDemo;

public sealed partial class MainWindowViewModel : ObservableObject
{
    private const string ImagePath = @"Assets\image1.jpg";

    [ObservableProperty]
    private double _counter;

    [ObservableProperty]
    private ImageSource? _winUI3Image;

    [ObservableProperty]
    private string? _message;

    public MainWindowViewModel()
    {
#if false
        // なぜかUIに反映されない。というかループが止まる？
        Task.Run(async () =>
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(10));
            while (await timer.WaitForNextTickAsync())
            {
                Counter += 0.01;
            }
        });
#endif
    }

    [RelayCommand]
    private void ClearImage()
    {
        WinUI3Image = null;
        Message = null;

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    [RelayCommand]
    private async Task LoadWpfImageAsync()
    {
        ClearImage();
        var sw = Stopwatch.StartNew();

        string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ImagePath);
#if false
        // こちらだとViewのコントロールにBindされるまで画像が読み込まれないっぽい。本関数を即抜けしちゃう。
        WinUI3Image = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
#else
        // こちらだとこの時点で画像が読み込まる。本関数を即抜けた時点で画像が表示される。
        WinUI3Image = await LoadImageAsync(imagePath);
#endif

        sw.Stop();
        Message = $"WinUI3 : {sw.ElapsedMilliseconds} msec";
    }

    private async static Task<BitmapImage?> LoadImageAsync(string imagePath)
    {
        try
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(imagePath);
            using Stream stream = await file.OpenStreamForReadAsync();
            BitmapImage bitmapImage = new();
            await bitmapImage.SetSourceAsync(stream.AsRandomAccessStream());
            return bitmapImage;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading image: {ex.Message}");
            return null;
        }
    }
}

