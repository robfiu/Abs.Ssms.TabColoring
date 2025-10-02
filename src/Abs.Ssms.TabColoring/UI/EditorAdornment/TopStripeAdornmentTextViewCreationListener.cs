using System;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Abs.Ssms.TabColoring.Services;
using Microsoft.VisualStudio.Shell;

namespace Abs.Ssms.TabColoring.UI.EditorAdornment
{
  [Export(typeof(IWpfTextViewCreationListener))]
  [ContentType("text")]
  [TextViewRole(PredefinedTextViewRoles.Document)]
  internal sealed class TopStripeAdornmentTextViewCreationListener : IWpfTextViewCreationListener
  {
    [Import]
    internal ITextDocumentFactoryService DocumentFactory = null;

    public void TextViewCreated(IWpfTextView textView)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      var adornment = new TopStripeAdornment(textView);

      // Use moniker (file path) as the key instead of IVsWindowFrame
      string key = null;
      ITextDocument doc;
      if (DocumentFactory != null && DocumentFactory.TryGetTextDocument(textView.TextBuffer, out doc) && doc != null)
      {
        if (!string.IsNullOrEmpty(doc.FilePath)) key = "mk:" + doc.FilePath;
      }

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
