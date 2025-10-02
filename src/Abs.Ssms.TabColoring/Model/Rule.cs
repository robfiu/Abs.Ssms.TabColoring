using System.Windows.Media;

namespace Abs.Ssms.TabColoring.Model
{
  public sealed class Rule
  {
    public string Pattern { get; set; }
    public RuleScope Scope { get; set; } = RuleScope.ServerOrDb;
    public Color Color { get; set; } = Colors.Transparent;

    public string ColorHex
    {
      get => $"#{Color.R:X2}{Color.G:X2}{Color.B:X2}";
      set
      {
        if (string.IsNullOrWhiteSpace(value)) return;
        var c = value.TrimStart('#');
        if (c.Length == 6)
        {
          byte r = System.Convert.ToByte(c.Substring(0, 2), 16);
          byte g = System.Convert.ToByte(c.Substring(2, 2), 16);
          byte b = System.Convert.ToByte(c.Substring(4, 2), 16);
          Color = Color.FromRgb(r, g, b);
        }
      }
    }
  }
}
