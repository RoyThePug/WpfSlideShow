using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfSlideShow.UserControl.UCWpfSlideShow;

public partial class UCWpfSlideShowControl : System.Windows.Controls.UserControl
{
    #region Dependency Property

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(UCWpfSlideShowControl), new PropertyMetadata(default(IEnumerable)));

    public IEnumerable ItemsSource
    {
        get => (IEnumerable) GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly DependencyProperty ItemTemplateProperty =
        DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(UCWpfSlideShowControl), new PropertyMetadata(default(DataTemplate)));

    public DataTemplate ItemTemplate
    {
        get => (DataTemplate) GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public static readonly DependencyProperty AddImageCommandProperty = DependencyProperty.Register(
        nameof(AddImageCommand), typeof(ICommand), typeof(UCWpfSlideShowControl), new PropertyMetadata(default(ICommand)));

    public ICommand AddImageCommand
    {
        get => (ICommand) GetValue(AddImageCommandProperty);
        set => SetValue(AddImageCommandProperty, value);
    }

    public static readonly DependencyProperty RemoveImageCommandProperty = DependencyProperty.Register(
        nameof(RemoveImageCommand), typeof(ICommand), typeof(UCWpfSlideShowControl), new PropertyMetadata(default(ICommand)));

    public ICommand RemoveImageCommand
    {
        get => (ICommand) GetValue(RemoveImageCommandProperty);
        set => SetValue(RemoveImageCommandProperty, value);
    }

    public static readonly DependencyProperty ChooseLastSelectedItemCommandProperty = DependencyProperty.Register(
        nameof(ChooseLastSelectedItemCommand), typeof(ICommand), typeof(UCWpfSlideShowControl), new PropertyMetadata(default(ICommand)));

    public ICommand ChooseLastSelectedItemCommand
    {
        get => (ICommand) GetValue(ChooseLastSelectedItemCommandProperty);
        set => SetValue(ChooseLastSelectedItemCommandProperty, value);
    }

    public static readonly DependencyProperty ChooseFirstSelectedItemCommandProperty = DependencyProperty.Register(
        nameof(ChooseFirstSelectedItemCommand), typeof(ICommand), typeof(UCWpfSlideShowControl), new PropertyMetadata(default(ICommand)));

    public ICommand ChooseFirstSelectedItemCommand
    {
        get => (ICommand) GetValue(ChooseFirstSelectedItemCommandProperty);
        set => SetValue(ChooseFirstSelectedItemCommandProperty, value);
    }

    public static readonly DependencyProperty RemoveLastItemCommandProperty = DependencyProperty.Register(
        nameof(RemoveLastItemCommand), typeof(ICommand), typeof(UCWpfSlideShowControl), new PropertyMetadata(default(ICommand)));

    public ICommand RemoveLastItemCommand
    {
        get => (ICommand) GetValue(RemoveLastItemCommandProperty);
        set => SetValue(RemoveLastItemCommandProperty, value);
    }

    #endregion

    public UCWpfSlideShowControl()
    {
        InitializeComponent();
    }
}