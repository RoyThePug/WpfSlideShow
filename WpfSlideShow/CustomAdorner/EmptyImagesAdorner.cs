using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfSlideShow.CustomAdorner;

public class EmptyImagesAdorner : Adorner
{
    #region Property

    public ContentPresenter ContentPresenter { get; set; }

    #endregion

    public EmptyImagesAdorner(UIElement adornedElement, DataTemplate template) : base(adornedElement)
    {
        ContentPresenter = new ContentPresenter
        {
            ContentTemplate = template
        };
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        ContentPresenter.Arrange(new Rect(0, 0,
            finalSize.Width, finalSize.Height));
        return ContentPresenter.RenderSize;
    }

    protected override Visual GetVisualChild(int index)
    {
        return ContentPresenter;
    }

    protected override int VisualChildrenCount => 1;
}