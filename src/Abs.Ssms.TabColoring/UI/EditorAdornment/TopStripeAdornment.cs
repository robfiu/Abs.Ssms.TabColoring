using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Editor;

namespace Abs.Ssms.TabColoring.UI.EditorAdornment
{
  internal sealed class TopStripeAdornment
  {
    private readonly IWpfTextView _view;
    private readonly IAdornmentLayer _layer;
    private readonly Border _stripe;

    public TopStripeAdornment(IWpfTextView view)
    {
      _view = view;
      _layer = view.GetAdornmentLayer("ABS.TopStripe");

      _stripe = new Border
      {
        Height = 3,
        Background = new SolidColorBrush(Colors.Transparent)
      };

      _view.ViewportWidthChanged += (_, __) => Position();
      _view.ViewportHeightChanged += (_, __) => Position();
      _view.LayoutChanged += (_, __) => Position();

      Position();
    }

    private void Position()
    {
      _layer.RemoveAllAdornments();
      Canvas.SetLeft(_stripe, _view.ViewportLeft);
      Canvas.SetTop(_stripe, _view.ViewportTop);
      _stripe.Width = _view.ViewportWidth;
      _layer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, _stripe, null);
    }

    public void SetColor(System.Windows.Media.Color c)
    {
      if (_stripe.Background is SolidColorBrush b) b.Color = c; else _stripe.Background = new SolidColorBrush(c);
    }
  }
}
