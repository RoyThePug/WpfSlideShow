using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using WpfSlideShow.CustomControl.ImageThumbnail;

namespace WpfSlideShow.CustomControl.ImageItemsContainer;

public class ImageItemsControl : Control
{
    #region Field

    private System.Windows.Controls.ItemsControl _itemsControl;

    private ScrollViewer _scrollViewer;

    private bool _scroolBusy;

    private int _selectedIndex;

    #endregion

    #region Property

    public ObservableCollection<ImageThumbnailControl> Items { get; }

    #endregion

    #region Dependency Property

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(ImageItemsControl), new PropertyMetadata(default(IEnumerable), OnItemsSourceChanged));

    public IEnumerable ItemsSource
    {
        get => (IEnumerable) GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ImageItemsControl control)
        {
            if (e.NewValue != null)
            {
                if (e.NewValue is INotifyCollectionChanged data)
                {
                    data.CollectionChanged += control.DataOnCollectionChanged;
                }

                foreach (var itemData in (IEnumerable) e.NewValue)
                {
                    if (itemData != null)
                    {
                        control.Add(itemData);
                    }
                }
            }
        }
    }

    public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
        nameof(SelectedItem), typeof(ImageSource), typeof(ImageItemsControl), new PropertyMetadata(default(ImageSource), OnSelectedItemChanged));

    public ImageSource SelectedItem
    {
        get => (ImageSource) GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ImageItemsControl control)
        {
            if (e.NewValue != null)
            {
                if (control.IsLoaded)
                {
                    var currentItem = control.Items.FirstOrDefault(x => x.Source.Equals(e.NewValue));

                    if (currentItem != null)
                    {
                        currentItem.BringIntoView();

                        control.UpdateCurrentItemSelectedState(currentItem);

                        control._selectedIndex = control.GetCurrentIndex();
                    }
                }
            }
        }
    }

    public static readonly DependencyProperty NewImageButtonTemplateProperty = DependencyProperty.Register(
        nameof(NewImageButtonTemplate), typeof(ControlTemplate), typeof(ImageItemsControl), new PropertyMetadata(default(ControlTemplate)));

    public ControlTemplate NewImageButtonTemplate
    {
        get => (ControlTemplate) GetValue(NewImageButtonTemplateProperty);
        set => SetValue(NewImageButtonTemplateProperty, value);
    }

    public static readonly DependencyProperty RemoveButtonTemplateProperty = DependencyProperty.Register(
        nameof(RemoveButtonTemplate), typeof(ControlTemplate), typeof(ImageItemsControl), new PropertyMetadata(default(ControlTemplate)));

    public ControlTemplate RemoveButtonTemplate
    {
        get => (ControlTemplate) GetValue(RemoveButtonTemplateProperty);
        set => SetValue(RemoveButtonTemplateProperty, value);
    }

    public static readonly DependencyProperty AddImageCommandProperty = DependencyProperty.Register(
        nameof(AddImageCommand), typeof(ICommand), typeof(ImageItemsControl), new PropertyMetadata(default(ICommand)));

    public ICommand AddImageCommand
    {
        get => (ICommand) GetValue(AddImageCommandProperty);
        set => SetValue(AddImageCommandProperty, value);
    }

    public static readonly DependencyProperty RemoveImageCommandProperty = DependencyProperty.Register(
        nameof(RemoveImageCommand), typeof(ICommand), typeof(ImageItemsControl), new PropertyMetadata(default(ICommand)));

    public ICommand RemoveImageCommand
    {
        get => (ICommand) GetValue(RemoveImageCommandProperty);
        set => SetValue(RemoveImageCommandProperty, value);
    }

    #endregion

    #region Constructor

    static ImageItemsControl()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageItemsControl), new FrameworkPropertyMetadata(typeof(ImageItemsControl)));
    }

    public ImageItemsControl()
    {
        Items = new ObservableCollection<ImageThumbnailControl>();
    }

    #endregion

    #region Method

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _itemsControl = Template.FindName("PART_ItemsControl", this) as System.Windows.Controls.ItemsControl;

        AddHandler(ImageThumbnailControl.IsActiveChangedEvent, new RoutedPropertyChangedEventHandler<ImageThumbnailControl>(RoutedPropertyChangedEventHandler));
        AddHandler(ImageThumbnailControl.AppearedEvent, new RoutedEventHandler(ImageThumbnailLoadedEventHandler));

        this.Loaded += OnLoaded;
    }

    protected virtual void Add(object dataContext)
    {
        var xqPanelItem = new ImageThumbnailControl();

        xqPanelItem.DataContext = dataContext;

        Items.Add(xqPanelItem);
        // Debug.WriteLine(_moveItemIndex);
    }

    protected virtual void Remove(object dataContext)
    {
        var currentItem = Items.FirstOrDefault(x => x.DataContext.Equals(dataContext));

        _selectedIndex = GetCurrentIndex();

        if (currentItem != null)
        {
            if (SelectedItem.Equals(currentItem.Source))
            {
                var prev = Items.LastOrDefault(x => Items.IndexOf(x) < _selectedIndex);
                var next = Items.LastOrDefault(x => Items.IndexOf(x) > _selectedIndex);

                if (next != null)
                {
                    SelectedItem = next.Source;
                }
                else if (prev != null)
                {
                    SelectedItem = prev.Source;
                }
                else
                {
                    SelectedItem = null;
                }
            }

            Items.Remove(currentItem);
        }
    }

    private int GetCurrentIndex()
    {
        if (!Items.Any())
        {
            return -1;
        }

        var currentItem = Items.FirstOrDefault(x => x.Source.Equals(SelectedItem));

        if (currentItem != null)
        {
            return Items.IndexOf(currentItem);
        }

        return -1;
    }

    private void UpdateCurrentItemSelectedState(ImageThumbnailControl imageThumbnailControl)
    {
        foreach (var imageControl in Items)
        {
            imageControl.IsSelected = false;
        }

        var searchItem = Items.FirstOrDefault(x => x.Equals(imageThumbnailControl));

        if (searchItem != null)
        {
            searchItem.IsSelected = true;
        }
    }

    #endregion

    #region Event Handler

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_itemsControl != null)
        {
            _scrollViewer = VisualTreeHelper.GetChild(_itemsControl, 0) as ScrollViewer;

            if (_scrollViewer != null)
            {
                AddHandler(RepeatButton.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(MouseButtonEventHandler));
                _scrollViewer.CommandBindings.Add(new CommandBinding(ScrollBar.LineLeftCommand, LineLeftCmdExecuted));
                _scrollViewer.CommandBindings.Add(new CommandBinding(ScrollBar.LineRightCommand, LineRightCmdExecuted));
            }
        }
    }

    private void ImageThumbnailLoadedEventHandler(object sender, RoutedEventArgs e)
    {
        if (IsLoaded && _scrollViewer != null && _scrollViewer.ActualWidth > 0)
        {
            var currentThumbnail = e.OriginalSource as ImageThumbnailControl;

            if (SelectedItem == null)
            {
                SelectedItem = currentThumbnail.Source;
            }
            else
            {
                if (SelectedItem.Equals(currentThumbnail.Source) && !currentThumbnail.IsSelected)
                {
                    UpdateCurrentItemSelectedState(currentThumbnail);
                }
            }
        }
    }

    private void RoutedPropertyChangedEventHandler(object sender, RoutedPropertyChangedEventArgs<ImageThumbnailControl> e)
    {
        var currentItem = e.NewValue;

        if (SelectedItem != null)
        {
            if (!SelectedItem.Equals(currentItem.Source))
            {
                SelectedItem = currentItem.Source;
            }
        }
        else
        {
            SelectedItem = currentItem.Source;
        }
    }

    private void DataOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            var data = e.NewItems[0];
            if (data != null)
            {
                Add(data);
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            var data = e.OldItems[0];
            if (data != null)
            {
                Remove(data);
            }
        }
    }

    private void LineLeftCmdExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (_scroolBusy || SelectedItem == null)
        {
            return;
        }

        _scroolBusy = true;

        if (_selectedIndex > 0)
        {
            _selectedIndex--;
        }
    }

    private void LineRightCmdExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (_scroolBusy || SelectedItem == null)
        {
            return;
        }

        _scroolBusy = true;

        if (_selectedIndex < Items.Count - 1)
        {
            _selectedIndex++;
        }
    }

    private void MouseButtonEventHandler(object sender, MouseButtonEventArgs e)
    {
        if (!_scroolBusy || SelectedItem == null)
        {
            return;
        }

        _scroolBusy = false;

        var currentThumbnail = Items[_selectedIndex];

        if (!SelectedItem.Equals(currentThumbnail.Source))
        {
            SelectedItem = currentThumbnail.Source;
        }
    }

    #endregion
}