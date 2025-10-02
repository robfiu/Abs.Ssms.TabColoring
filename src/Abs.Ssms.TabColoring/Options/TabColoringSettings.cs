using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Abs.Ssms.TabColoring.Model;

namespace Abs.Ssms.TabColoring.Options
{
  public sealed class TabColoringSettings
  {
    public bool EnableTabBackground { get; set; } = true;
    public bool EnableTopStripe { get; set; } = true;
    public bool EnableTitlePrefix { get; set; } = true;
    public string TitlePrefix { get; set; } = "● ";
    public bool UseRegex { get; set; } = false;

    public string RulesSerialized { get; set; } = "prod|ServerOrDb|#E63946\nstage|ServerOrDb|#FCBF49\ndev|ServerOrDb|#457B9D";

    public List<Rule> GetRules()
    {
      var rules = new List<Rule>();
      if (string.IsNullOrWhiteSpace(RulesSerialized)) return rules;
      var lines = RulesSerialized.Replace("\r", string.Empty).Split('\n');
      foreach (var line in lines)
      {
        var parts = line.Split('|');
        if (parts.Length < 3) continue;
        var pattern = parts[0];
        if (!Enum.TryParse(parts[1], out RuleScope scope)) scope = RuleScope.ServerOrDb;
        var color = ParseColor(parts[2]);
        rules.Add(new Rule { Pattern = pattern, Scope = scope, Color = color });
      }
      return rules;
    }

    public void SetRules(IEnumerable<Rule> rules)
    {
      var sb = new StringBuilder();
      foreach (var r in rules)
      {
        sb.Append(r.Pattern?.Replace("|", "/")).Append('|')
          .Append(r.Scope).Append('|')
          .Append(ColorToHex(r.Color)).Append('\n');
      }
      RulesSerialized = sb.ToString().TrimEnd('\n');
    }

    private static Color ParseColor(string hex)
    {
      if (string.IsNullOrWhiteSpace(hex)) return Colors.Transparent;
      hex = hex.Trim();
      if (hex.StartsWith("#")) hex = hex.Substring(1);
      if (hex.Length == 6)
      {
        byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
        return Color.FromRgb(r, g, b);
      }
      if (hex.Length == 8)
      {
        byte a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
        byte r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
        return Color.FromArgb(a, r, g, b);
      }
      return Colors.Transparent;
    }

    public static string ColorToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
  }
}
