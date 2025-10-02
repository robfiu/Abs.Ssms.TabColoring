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
      var frame = VsShellUtilities.GetTextViewFrame(vsTextView) as IVsWindowFrame;
      var key = ColorRegistry.GetFrameKey(frame);
      if (key != null && ColorRegistry.TryGetColor(key, out Color c)) adornment.SetColor(c);

      ColorRegistry.ColorChanged += (_, changedKey) => { if (key == changedKey && ColorRegistry.TryGetColor(key, out Color c2)) adornment.SetColor(c2); };
    }
  }
}
