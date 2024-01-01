using System.IO;
using System.Windows.Media.Imaging;

namespace WpfSkiaImageDemo;

public static class BitmapSourceEx
{
    public static BitmapSource? ToBitmapSource(string filePath)
    {
        if (!File.Exists(filePath))
            return null;

        BitmapSource bitmap;
        try
        {
            using var stream = File.OpenRead(filePath);
            var image = new BitmapImage();
            image.BeginInit();
            image.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;  // カメラ撮って出しjpg画素値がbmpとjpgずれる対策!!
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze();
            bitmap = image;
        }
        catch (NotSupportedException)
        {
            return null;    // ファイルサイズゼロの画像だと入ってきます(ここまでに弾いていません)
        }

        return bitmap;
    }
}
