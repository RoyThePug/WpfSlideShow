using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfSlideShow.CustomControl.ImageItemsContainer;

namespace WpfSlideShow.CustomControl.ImageThumbnail;

public class ImageThumbnailControl : Control
{
    #region Field

    private Grid _part_Root;

    private ImageItemsControl _parentControl;

    #endregion

    #region Dependency Property

    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        nameof(Source), typeof(ImageSource), typeof(ImageThumbnailControl), new PropertyMetadata(default(ImageSource)));

    public ImageSource Source
    {
        get => (ImageSource) GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
        nameof(IsSelected), typeof(bool), typeof(ImageThumbnailControl), new PropertyMetadata(default(bool), IsSelectedChanged));

    public bool IsSelected
    {
        get => (bool) GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    private static void IsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ImageThumbnailControl control)
        {
            control.UpdateSelectedState((bool) e.NewValue);
        }
    }

    #endregion

    #region Routed Event

    public static readonly RoutedEvent IsActiveChangedEvent = EventManager.RegisterRoutedEvent(nameof(IsActiveChanged), RoutingStrategy.Bubble,
        typeof(RoutedPropertyChangedEventHandler<ImageThumbnailControl>),
        typeof(ImageThumbnailControl));

    public event RoutedPropertyChangedEventHandler<ImageThumbnailControl> IsActiveChanged
    {
        add => AddHandler(IsActiveChangedEvent, value);
        remove => RemoveHandler(IsActiveChangedEvent, value);
    }

    public static readonly RoutedEvent AppearedEvent = EventManager.RegisterRoutedEvent(nameof(Appeared), RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(ImageThumbnailControl));

    public event RoutedEventHandler Appeared
    {
        add => AddHandler(AppearedEvent, value);
        remove => RemoveHandler(AppearedEvent, value);
    }

    #endregion

    #region Constructor

    static ImageThumbnailControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageThumbnailControl), new FrameworkPropertyMetadata(typeof(ImageThumbnailControl)));
    }

    public ImageThumbnailControl()
    {
    }

    #endregion

    #region Method

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _part_Root = Template.FindName("ROOT_Element", this) as Grid;

        if (_part_Root != null)
        {
            _part_Root.PreviewMouseLeftButtonDown += Part_AnimatedContainerOnPreviewMouseLeftButtonDown;
        }

        _parentControl = FindParent<ImageItemsControl>(this);

        Loaded += OnLoaded;
        
        UpdateSelectedState(IsSelected);
    }

    protected virtual void UpdateSelectedState(bool value)
    {
        if (value)
        {
            VisualStateManager.GoToState(this, "IsSelectedState", false);
        }
        else
        {
            VisualStateManager.GoToState(this, "Normal", false);
        }
    }

    protected T FindParent<T>(DependencyObject dependencyObject) where T : DependencyObject
    {
        var parent = VisualTreeHelper.GetParent(dependencyObject);

        if (parent == null) return null;

        var parentT = parent as T;
        return parentT ?? FindParent<T>(parent);
    }

    #endregion

    #region Handler
    
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(AppearedEvent));
    }

    private void Part_AnimatedContainerOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!IsSelected)
        {
            IsSelected = true;

            RaiseEvent(new RoutedPropertyChangedEventArgs<ImageThumbnailControl>(null, this, IsActiveChangedEvent));
        }
    }

    #endregion
}