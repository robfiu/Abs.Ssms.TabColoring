using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Abs.Ssms.TabColoring.Model;
using Abs.Ssms.TabColoring.Options;
using Abs.Ssms.TabColoring.UI;

namespace Abs.Ssms.TabColoring.Services
{
  internal sealed class DocumentTabColorManager : IVsShellPropertyEvents
  {
    private readonly IServiceProvider _sp;
    private readonly IVsShell _vsShell;
    private readonly IVsUIShell _uiShell;
    private readonly IVsMonitorSelection _mon;
    private uint _cookie;

    public DocumentTabColorManager(IServiceProvider sp, IVsShell vsShell, IVsUIShell uiShell, IVsMonitorSelection mon)
    {
      _sp = sp; _vsShell = vsShell; _uiShell = uiShell; _mon = mon;
    }

    public void Hook()
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      if (_vsShell != null)
      {
        _vsShell.AdviseShellPropertyChanges(this, out _cookie);
      }
      RefreshAllOpenFrames();
    }

    public void Unhook()
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      if (_cookie != 0 && _vsShell != null) _vsShell.UnadviseShellPropertyChanges(_cookie);
    }

    public void RefreshAllOpenFrames()
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      foreach (var frame in EnumerateDocumentFrames())
      {
        TryColorFrame(frame);
      }
    }

    private IEnumerable<IVsWindowFrame> EnumerateDocumentFrames()
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      IEnumWindowFrames e;
      _uiShell.GetDocumentWindowEnum(out e);
      var arr = new IVsWindowFrame[1];
      uint fetched;
      while (e != null && e.Next(1, arr, out fetched) == VSConstants.S_OK && fetched == 1)
        yield return arr[0];
    }

    private void TryColorFrame(IVsWindowFrame frame)
    {
      ThreadHelper.ThrowIfNotOnUIThread();

      var s = SettingsService.Current;
      var conn = ConnectionInfoResolver.TryGetConnectionForFrame(frame);
      var server = conn.server;
      var db = conn.database;

      var color = PickColor(s, server, db);
      var key = ColorRegistry.GetFrameKey(frame);
      if (key != null && color.HasValue) ColorRegistry.SetColor(key, color.Value);

      bool colored = false;
      if (color.HasValue && s.EnableTabBackground)
      {
        colored = TabVisualHelper.TryColorTabBackground(frame, color.Value);
      }

      if ((!colored || !s.EnableTabBackground) && s.EnableTitlePrefix)
      {
        TabVisualHelper.PrefixTitle(frame, s.TitlePrefix);
      }
    }

    private Color? PickColor(TabColoringSettings s, string server, string db)
    {
      var rules = s.GetRules();
      if (rules == null || rules.Count == 0) return null;

      string hayServer = (server ?? string.Empty).ToLowerInvariant();
      string hayDb = (db ?? string.Empty).ToLowerInvariant();
      string both = (hayServer + " " + hayDb).Trim();

      foreach (var r in rules)
      {
        bool match = false;
        string p = (r.Pattern ?? string.Empty).ToLowerInvariant();
        if (s.UseRegex)
        {
          try
          {
            var rx = new Regex(r.Pattern, RegexOptions.IgnoreCase);
            if (r.Scope == RuleScope.ServerOnly) match = rx.IsMatch(hayServer);
            else if (r.Scope == RuleScope.DatabaseOnly) match = rx.IsMatch(hayDb);
            else match = rx.IsMatch(both);
          }
          catch { }
        }
        else
        {
          if (r.Scope == RuleScope.ServerOnly) match = hayServer.Contains(p);
          else if (r.Scope == RuleScope.DatabaseOnly) match = hayDb.Contains(p);
          else match = both.Contains(p);
        }

        if (match) return r.Color;
      }
      return null;
    }

    public int OnShellPropertyChange(int propid, object var)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      RefreshAllOpenFrames();
      return VSConstants.S_OK;
    }
  }
}
