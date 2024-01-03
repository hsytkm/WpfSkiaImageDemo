using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

namespace WinUI3ImageDemo;

public sealed partial class MainWindowViewModel : ObservableObject
{
    private static string ImagePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Assets\image1.jpg");

    [ObservableProperty]
    private double _counter;

    [ObservableProperty]
    private BitmapImage? _bitmapImage;

    [ObservableProperty]
    private WriteableBitmap? _writeableBitmap;

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
        BitmapImage = null;
        WriteableBitmap = null;
        Message = null;

        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    [RelayCommand]
    private async Task LoadBitmapImageAsync(CancellationToken cancellationToken = default)
    {
        ClearImage();
        var sw = Stopwatch.StartNew();

#if false
        // こちらだとViewのコントロールにBindされるまで画像が読み込まれないっぽい。本関数を即抜けしちゃう。
        BitmapImage = new BitmapImage(new Uri(ImagePath, UriKind.Absolute));
#else
        // こちらだとこの時点で画像が読み込まれて、本関数を即抜けた時点で画像が表示されます。
        // BitmapImage だと画素値をポインタで読み込む術がなさそうです。
        BitmapImage = await LoadBitmapImageCoreAsync(ImagePath, cancellationToken);
#endif

        sw.Stop();
        Message = $"BitmapImage : {sw.ElapsedMilliseconds} msec";
    }

    [RelayCommand]
    private async Task LoadWriteableBitmapAsync(CancellationToken cancellationToken = default)
    {
        ClearImage();
        var sw = Stopwatch.StartNew();

        // こちらだとこの時点で画像が読み込まれて、本関数を即抜けた時点で画像が表示されます。
        // WriteableBitmap なら画素値をポインタで読み込めそうですが、BitmapImage より1割くらい遅そうです。
        var image = await LoadWriteableBitmapCoreAsync(ImagePath, cancellationToken);
        WriteableBitmap = image;
        //await GetPixelChannelsAverageAsync(image!, cancellationToken);

        sw.Stop();
        Message = $"WriteableBitmap : {sw.ElapsedMilliseconds} msec";
    }

    // BitmapImage だと画素値をポインタで読み込む術がなさそうです。
    private static async Task<BitmapImage?> LoadBitmapImageCoreAsync(string imagePath, CancellationToken cancellationToken)
    {
        try
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(imagePath).AsTask(cancellationToken);
            using IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read).AsTask(cancellationToken);
            BitmapImage image = new();
            await image.SetSourceAsync(stream).AsTask(cancellationToken);
            return image;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading image: {ex.Message}");
            return null;
        }
    }

    // WriteableBitmap なら画素値をポインタで読み込めそうです。(BitmapImage より1割くらい遅そうです)
    private static async Task<WriteableBitmap?> LoadWriteableBitmapCoreAsync(string imagePath, CancellationToken cancellationToken)
    {
        try
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(imagePath).AsTask(cancellationToken);
            using IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read).AsTask(cancellationToken);
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream).AsTask(cancellationToken);
            WriteableBitmap image = new((int)decoder.PixelWidth, (int)decoder.PixelHeight);
            await image.SetSourceAsync(stream).AsTask(cancellationToken);
            return image;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading image: {ex.Message}");
            return null;
        }
    }

    // 画素値読み出しのお試し
    private static async Task GetPixelChannelsAverageAsync(WriteableBitmap bitmap, CancellationToken cancellationToken)
    {
        // アロケ削減のため、画素を1行ごとに読み込むメソッドは提供されてなさそうですが、
        // メモリに隙間なく詰まっているようなので、自分で良い感じに読めばアロケーションを削減できそうです。
        using Stream stream = bitmap.PixelBuffer.AsStream();
        var pixelArray = new byte[stream.Length];
        _ = await stream.ReadAsync(pixelArray, cancellationToken);

        // WriteableBitmap はBGRA固定らしい(BingChat談)
        ulong sumB = 0, sumG = 0, sumR = 0, sumA = 0;
        for (int i = 0; i < pixelArray.Length; i += 4)
        {
            sumB += pixelArray[i];
            sumG += pixelArray[i + 1];
            sumR += pixelArray[i + 2];
            sumA += pixelArray[i + 3];
        }

        double pixels = bitmap.PixelWidth * bitmap.PixelHeight;
        double aveB = sumB / pixels;
        double aveG = sumG / pixels;
        double aveR = sumR / pixels;
        double aveA = sumA / pixels;
        Debug.WriteLine($"Ave: B={aveB:f2}, G={aveG:f2}, R={aveR:f2}, A={aveA:f2}");
    }
}

