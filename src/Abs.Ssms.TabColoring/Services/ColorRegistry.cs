using System;
using System.Collections.Concurrent;
using System.Windows.Media;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Abs.Ssms.TabColoring.Services
{
  public static class ColorRegistry
  {
    private static readonly ConcurrentDictionary<string, Color> _byKey = new ConcurrentDictionary<string, Color>();

    public static event EventHandler<string> ColorChanged;

    public static string GetFrameKey(IVsWindowFrame frame)
    {
      if (frame == null) return null;
      object mk, cap;
      if (frame.GetProperty((int)__VSFPROPID.VSFPROPID_pszMkDocument, out mk) == Microsoft.VisualStudio.VSConstants.S_OK && mk is string moniker && !string.IsNullOrEmpty(moniker))
        return "mk:" + moniker;
      if (frame.GetProperty((int)__VSFPROPID.VSFPROPID_OwnerCaption, out cap) == Microsoft.VisualStudio.VSConstants.S_OK && cap is string caption && !string.IsNullOrEmpty(caption))
        return "cap:" + caption;
      return null;
    }

    public static void SetColor(string key, Color color)
    {
      if (string.IsNullOrEmpty(key)) return;
      _byKey[key] = color;
      var h = ColorChanged;
      if (h != null) h(null, key);
    }

    public static bool TryGetColor(string key, out Color color) { return _byKey.TryGetValue(key, out color); }
  }
}
