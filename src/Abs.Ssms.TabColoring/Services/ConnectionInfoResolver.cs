using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Abs.Ssms.TabColoring.Services
{
  public static class ConnectionInfoResolver
  {
    private static readonly Regex[] Patterns = new[]
    {
      new Regex(@"@(?<server>[A-Za-z0-9_.-]+)\s*\((?<db>[^)]+)\)", RegexOptions.Compiled),
      new Regex(@"(?<db>[A-Za-z0-9_\-\$]+)\s+on\s+(?<server>[A-Za-z0-9_.-]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase),
    };

    public static (string server, string database) TryGetConnectionForFrame(IVsWindowFrame frame)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      if (frame == null) return (null, null);
      if (frame.GetProperty((int)__VSFPROPID.VSFPROPID_OwnerCaption, out object capObj) == VSConstants.S_OK && capObj is string cap)
      {
        foreach (var rx in Patterns)
        {
          var m = rx.Match(cap);
          if (m.Success)
          {
            var server = m.Groups["server"].Value;
            var db = m.Groups["db"].Value;
            return (server, db);
          }
        }
      }

      // TODO: advanced approach via SSMS services if available.
      return (null, null);
    }
  }
}
