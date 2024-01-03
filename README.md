# WpfSkiaImageDemo

### What I Tried

I heard that drawing with SkiaSharp is fast, so I tried displaying an image file for the time being.

### Result

Skia was slower than WPF's BitmapSource.

*It might be faster with Skia when overlaying geometries.

### Measurement

Image Width=10368, Height=7776

||Processing time (msec)|Memory usage|
|--|--|--|
|WPF - BitmapImage|454|Medium|
|WPF - SkiaSharp|1058|Large|
|Avalonia (SkiaSharp)|810|Large|
|WinUI3 v1.4 - BitmapImage|470|Small|
|WinUI3 v1.4 - WriteableBitmap|500|Large|

I found that WPF's BitmapImage is the fastest, but WinUI3's BitmapImage consumes less memory.



![demo.gif](https://github.com/hsytkm/WpfSkiaImageDemo/blob/main/WpfSkiaImageDemo/demo.gif)

### References

[SkiaSharpを使用しての地図描画 #.NET - Qiita](https://qiita.com/ingen084/items/8c4492bfb3cc50129507)

[kekyo/SkiaImageView: A control for easy way showing SkiaSharp-based image objects onto WPF/XF/.NET MAUI/Avalonia applications.](https://github.com/kekyo/SkiaImageView)

