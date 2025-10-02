using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Abs.Ssms.TabColoring.UI
{
  public static class TabVisualHelper
  {
    public static bool TryColorTabBackground(IVsWindowFrame frame, Color color)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      try
      {
        if (frame.GetProperty((int)__VSFPROPID.VSFPROPID_OwnerCaption, out object capObj) != VSConstants.S_OK || capObj is not string caption)
          return false;

        foreach (Window w in Application.Current.Windows)
        {
          var tabItem = FindTabItemByHeader(w, caption);
          if (tabItem != null)
          {
            if (tabItem.Background is SolidColorBrush b && b.Color == color) return true;
            tabItem.Background = new SolidColorBrush(color);
            tabItem.BorderBrush = new SolidColorBrush(color);
            return true;
          }
        }
      }
      catch { }
      return false;
    }

    public static void PrefixTitle(IVsWindowFrame frame, string prefix)
    {
      ThreadHelper.ThrowIfNotOnUIThread();
      if (string.IsNullOrEmpty(prefix)) return;
      if (frame.GetProperty((int)__VSFPROPID.VSFPROPID_OwnerCaption, out object capObj) == VSConstants.S_OK && capObj is string cap)
      {
        if (!cap.StartsWith(prefix, StringComparison.Ordinal))
        {
          frame.SetProperty((int)__VSFPROPID.VSFPROPID_OwnerCaption, prefix + cap);
        }
      }
    }

    private static TabItem FindTabItemByHeader(DependencyObject root, string header)
    {
      if (root == null) return null;
      int count = VisualTreeHelper.GetChildrenCount(root);
      for (int i = 0; i < count; i++)
      {
        var child = VisualTreeHelper.GetChild(root, i);
        if (child is TabItem ti)
        {
          var text = ti.Header as string;
          if (!string.IsNullOrEmpty(text) && text.Contains(header)) return ti;
        }
        var found = FindTabItemByHeader(child, header);
        if (found != null) return found;
      }
      return null;
    }
  }
}
