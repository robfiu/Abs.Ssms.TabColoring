using System;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Settings;
using Abs.Ssms.TabColoring.Options;

namespace Abs.Ssms.TabColoring.Services
{
  public static class SettingsService
  {
    private const string CollectionPath = "ABS.TabColoring";

    private static AsyncPackage _package;
    public static TabColoringSettings Current { get; private set; } = new TabColoringSettings();

    public static event EventHandler SettingsChanged;

    public static void Initialize(AsyncPackage package)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      _package = package;
      Reload();
    }

    public static void Reload()
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      var mgr = new ShellSettingsManager(_package);
      var store = mgr.GetReadOnlySettingsStore(SettingsScope.UserSettings);

      var s = new TabColoringSettings();
      if (store.CollectionExists(CollectionPath))
      {
        s.EnableTabBackground = store.GetBoolean(CollectionPath, nameof(s.EnableTabBackground), s.EnableTabBackground);
        s.EnableTopStripe = store.GetBoolean(CollectionPath, nameof(s.EnableTopStripe), s.EnableTopStripe);
        s.EnableTitlePrefix = store.GetBoolean(CollectionPath, nameof(s.EnableTitlePrefix), s.EnableTitlePrefix);
        s.TitlePrefix = store.GetString(CollectionPath, nameof(s.TitlePrefix), s.TitlePrefix);
        s.UseRegex = store.GetBoolean(CollectionPath, nameof(s.UseRegex), s.UseRegex);
        s.RulesSerialized = store.GetString(CollectionPath, nameof(s.RulesSerialized), s.RulesSerialized);
      }

      Current = s;
      SettingsChanged?.Invoke(null, EventArgs.Empty);
    }

    public static void Save()
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      var mgr = new ShellSettingsManager(_package);
      var store = mgr.GetWritableSettingsStore(SettingsScope.UserSettings);
      if (!store.CollectionExists(CollectionPath)) store.CreateCollection(CollectionPath);

      var s = Current;
      store.SetBoolean(CollectionPath, nameof(s.EnableTabBackground), s.EnableTabBackground);
      store.SetBoolean(CollectionPath, nameof(s.EnableTopStripe), s.EnableTopStripe);
      store.SetBoolean(CollectionPath, nameof(s.EnableTitlePrefix), s.EnableTitlePrefix);
      store.SetString(CollectionPath, nameof(s.TitlePrefix), s.TitlePrefix ?? string.Empty);
      store.SetBoolean(CollectionPath, nameof(s.UseRegex), s.UseRegex);
      store.SetString(CollectionPath, nameof(s.RulesSerialized), s.RulesSerialized ?? string.Empty);

      SettingsChanged?.Invoke(null, EventArgs.Empty);
    }
  }
}
