using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace WinUI3ImageDemo;

public class ImageFileInfo(
    ImageProperties properties,
    StorageFile imageFile,
    string name,
    string type
    ) : ObservableObject
{
    public StorageFile ImageFile { get; } = imageFile;

    public ImageProperties ImageProperties { get; } = properties;

    public async Task<BitmapImage> GetImageSourceAsync()
    {
        using IRandomAccessStream fileStream = await ImageFile.OpenReadAsync();

        // Create a bitmap to be the image source.
        BitmapImage bitmapImage = new();
        bitmapImage.SetSource(fileStream);

        return bitmapImage;
    }

    public async Task<BitmapImage> GetImageThumbnailAsync()
    {
        using StorageItemThumbnail thumbnail = await ImageFile.GetThumbnailAsync(ThumbnailMode.PicturesView);

        // Create a bitmap to be the image source.
        var bitmapImage = new BitmapImage();
        bitmapImage.SetSource(thumbnail);

        return bitmapImage;
    }

    public string ImageName { get; } = name;

    public string ImageFileType { get; } = type;

    public string ImageDimensions => $"{ImageProperties.Width} x {ImageProperties.Height}";

    public string ImageTitle
    {
        get => string.IsNullOrEmpty(ImageProperties.Title) ? ImageName : ImageProperties.Title;
        set
        {
            if (ImageProperties.Title != value)
            {
                ImageProperties.Title = value;
                _ = ImageProperties.SavePropertiesAsync();
                OnPropertyChanged();
            }
        }
    }
}
