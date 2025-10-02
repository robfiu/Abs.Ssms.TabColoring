using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Abs.Ssms.TabColoring.Services;
using Abs.Ssms.TabColoring.UI;
using Abs.Ssms.TabColoring.Options;
using Task = System.Threading.Tasks.Task;

namespace Abs.Ssms.TabColoring.Package
{
  [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
  [InstalledProductRegistration("ABS SSMS Tab Coloring", "Colors editor tabs by connection", "0.1.0")]
  [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
  [ProvideOptionPage(typeof(TabColoringOptionsPage), "ABS", "Tab Coloring", 0, 0, true)]
  // IMPORTANT: VS SDK 17.x expects 'isToolsOptionPage' (bool) as the last arg
  [ProvideProfile(typeof(TabColoringOptionsPage), "ABS", "Tab Coloring", 0, 0, isToolsOptionPage: true)]
  [Guid(PackageGuidString)]
  public sealed class TabColoringPackage : AsyncPackage
  {
    public const string PackageGuidString = "B9B0B9D9-8D77-4A2A-9C1E-5E1E5E2BAA90";

    private DocumentTabColorManager _manager;

    protected override async Task InitializeAsync(CancellationToken ct, IProgress<ServiceProgressData> progress)
    {
      await JoinableTaskFactory.SwitchToMainThreadAsync(ct);

      SettingsService.Initialize(this);

      var shell = await GetServiceAsync(typeof(SVsUIShell)) as IVsUIShell;
      var monitor = await GetServiceAsync(typeof(SVsUIShellMonitorSelection)) as IVsMonitorSelection;

      _manager = new DocumentTabColorManager(this, shell, monitor);
      _manager.Hook();

      SettingsService.SettingsChanged += (_, __) => _manager.RefreshAllOpenFrames();
    }

    protected override void Dispose(bool disposing)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      _manager?.Unhook();
      base.Dispose(disposing);
    }
  }
}
