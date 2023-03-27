using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfSlideShow.ViewModel;

public partial class PictureViewModel : ObservableObject
{
    #region Property

    [ObservableProperty] int id;
    [ObservableProperty] string path;
    [ObservableProperty] ImageSource imageSource;

    #endregion

    public PictureViewModel(int id, string path)
    {
        Id = id;
        Path = path;

        SetImageData(path);
     //SetImageData(File.ReadAllBytes(path));
    }

    public PictureViewModel(int id, byte[] data)
    {
        Id = id;
        Path = path;
        SetImageData(data);
    }

    private void SetImageData(byte[] data)
    {
        var source = new BitmapImage();
        source.BeginInit();
        source.StreamSource = new MemoryStream(data);
        source.EndInit();

        // use public setter
        ImageSource = source;
    }

    private void SetImageData(string data)
    {
        var uri = new Uri(data, UriKind.Relative);
        imageSource = new BitmapImage(uri);
    }
}