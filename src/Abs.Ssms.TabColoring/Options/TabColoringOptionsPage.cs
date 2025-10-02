using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using Abs.Ssms.TabColoring.Services;

namespace Abs.Ssms.TabColoring.Options
{
  [Guid("9B7A6E44-8E6D-4C30-9E3B-4F195B28B0A2")]
  public class TabColoringOptionsPage : DialogPage
  {
    private RulesOptionsControl _control;

    protected override IWin32Window Window
    {
      get
      {
        if (_control == null)
        {
          _control = new RulesOptionsControl();
          _control.LoadFrom(SettingsService.Current);
        }
        return _control;
      }
    }

    public override void LoadSettingsFromStorage()
    {
      base.LoadSettingsFromStorage();
      SettingsService.Reload();
      _control?.LoadFrom(SettingsService.Current);
    }

    public override void SaveSettingsToStorage()
    {
      if (_control != null)
      {
        _control.SaveTo(SettingsService.Current);
        SettingsService.Save();
      }
      base.SaveSettingsToStorage();
    }
  }
}
