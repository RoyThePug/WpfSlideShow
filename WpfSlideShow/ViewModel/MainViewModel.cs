using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace WpfSlideShow.ViewModel;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty] ImageSource selectedImage;
    
    public ObservableCollection<PictureViewModel> Pictures { get; }

    public bool CanSelectedItemExecute => Pictures.Any();

    public MainViewModel()
    {
        Pictures = new ObservableCollection<PictureViewModel>
        {
             new PictureViewModel(1, @"/WpfSlideShow;component/Image/1.png"),
             new PictureViewModel(2, @"/WpfSlideShow;component/Image/2.png"),
             new PictureViewModel(3, @"/WpfSlideShow;component/Image/3.png"),
            new PictureViewModel(4, @"/WpfSlideShow;component/Image/4.png"),
            new PictureViewModel(5, @"/WpfSlideShow;component/Image/5.png"),
            new PictureViewModel(6, @"/WpfSlideShow;component/Image/6.png"),
            new PictureViewModel(7, @"/WpfSlideShow;component/Image/7.png"),
            new PictureViewModel(8, @"/WpfSlideShow;component/Image/8.png")
        };

        if (Pictures.Any())
        {
            SelectedImage = Pictures.FirstOrDefault()!.ImageSource;
        }
    }

    #region Command

    [RelayCommand]
    async Task AddImageAsync()
    {
        try
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                Pictures.Add(new PictureViewModel(Pictures.Count + 1, File.ReadAllBytes($"{openFileDialog.FileName}")));
               
              //  SelectedImage = Pictures.LastOrDefault().ImageSource;

                ChooseFirstSelectedItemCommand.NotifyCanExecuteChanged();
                ChooseLastSelectedItemCommand.NotifyCanExecuteChanged();
                RemoveLastItemCommand.NotifyCanExecuteChanged();
            }
        }
        catch (Exception e)
        {
        }
    }

    [RelayCommand]
    async Task RemoveImageAsync()
    {
        if (SelectedImage != null)
        {
            var currentItem = Pictures.FirstOrDefault(x => x.ImageSource.Equals(SelectedImage));

            if (currentItem != null)
            {
                Pictures.Remove(currentItem);
            }

            ChooseFirstSelectedItemCommand.NotifyCanExecuteChanged();
            ChooseLastSelectedItemCommand.NotifyCanExecuteChanged();
            RemoveLastItemCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand(CanExecute = nameof(CanSelectedItemExecute))]
    async Task ChooseLastSelectedItemAsync()
    {
        SelectedImage = Pictures.LastOrDefault()!.ImageSource;
    }

    [RelayCommand(CanExecute = nameof(CanSelectedItemExecute))]
    async Task ChooseFirstSelectedItemAsync()
    {
        SelectedImage = Pictures.FirstOrDefault()!.ImageSource;
    }
    
    [RelayCommand(CanExecute = nameof(CanSelectedItemExecute))]
    async Task RemoveLastItemAsync()
    {
        var currentItem = Pictures.LastOrDefault();

        if (currentItem != null)
        {
            Pictures.Remove(currentItem);
        }
    }

    #endregion
}