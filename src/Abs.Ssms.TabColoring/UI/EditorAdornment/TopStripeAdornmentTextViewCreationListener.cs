using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Editor;
using Abs.Ssms.TabColoring.Services;

namespace Abs.Ssms.TabColoring.UI.EditorAdornment
{
  [Export(typeof(IWpfTextViewCreationListener))]
  [ContentType("text")]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  internal sealed class TopStripeAdornmentTextViewCreationListener : IWpfTextViewCreationListener
  {
    [Import]
    internal IVsEditorAdaptersFactoryService AdaptersFactory = null;

    public void TextViewCreated(IWpfTextView textView)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      var adornment = new TopStripeAdornment(textView);

      var vsTextView = AdaptersFactory.GetViewAdapter(textView);
      var frame = Microsoft.VisualStudio.Shell.VsShellUtilities.GetWindowFrame(vsTextView) as IVsWindowFrame;
      var key = ColorRegistry.GetFrameKey(frame);
      if (key != null)
      {
        Color c;
        if (ColorRegistry.TryGetColor(key, out c)) adornment.SetColor(c);
        ColorRegistry.ColorChanged += (sender, changedKey) =>
        {
          if (key == changedKey)
          {
            Color c2;
            if (ColorRegistry.TryGetColor(key, out c2)) adornment.SetColor(c2);
          }
        };
      }
    }
  }
}
