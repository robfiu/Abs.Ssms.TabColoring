using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Abs.Ssms.TabColoring.Model;
using Abs.Ssms.TabColoring.Options;
using System.Windows.Media;

namespace Abs.Ssms.TabColoring.Options
{
  public class RulesOptionsControl : UserControl
  {
    private CheckBox chkTabBg;
    private CheckBox chkTopStripe;
    private CheckBox chkTitlePrefix;
    private TextBox txtPrefix;
    private CheckBox chkRegex;
    private DataGridView grid;
    private Button btnAdd, btnRemove, btnColor;

    private BindingList<Rule> _binding = new BindingList<Rule>();

    public RulesOptionsControl()
    {
      InitializeComponent();
    }

    private void InitializeComponent()
    {
      Dock = DockStyle.Fill;

      chkTabBg = new CheckBox { Text = "Enable tab background", Left = 10, Top = 10, Width = 220 };
      chkTopStripe = new CheckBox { Text = "Enable top stripe (adornment)", Left = 10, Top = 35, Width = 260 };
      chkTitlePrefix = new CheckBox { Text = "Enable title prefix", Left = 10, Top = 60, Width = 220 };
      chkRegex = new CheckBox { Text = "Use regex patterns", Left = 10, Top = 85, Width = 220 };
      txtPrefix = new TextBox { Left = 240, Top = 58, Width = 60, Text = "● " };

      grid = new DataGridView { Left = 10, Top = 120, Width = 560, Height = 240, AutoGenerateColumns = false, AllowUserToAddRows = false };
      grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Pattern", DataPropertyName = nameof(Rule.Pattern), Width = 280 });
      grid.Columns.Add(new DataGridViewComboBoxColumn { HeaderText = "Scope", DataPropertyName = nameof(Rule.Scope), DataSource = Enum.GetValues(typeof(RuleScope)) });
      grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Color (#RRGGBB)", DataPropertyName = nameof(Rule.ColorHex), Width = 140 });

      btnAdd = new Button { Text = "Add", Left = 10, Top = 370, Width = 80 };
      btnRemove = new Button { Text = "Remove", Left = 100, Top = 370, Width = 80 };
      btnColor = new Button { Text = "Pick Color", Left = 190, Top = 370, Width = 100 };

      btnAdd.Click += (_, __) => _binding.Add(new Rule { Pattern = "dev", Scope = RuleScope.ServerOrDb, Color = Colors.SteelBlue });
      btnRemove.Click += (_, __) => { if (grid.CurrentRow?.DataBoundItem is Rule r) _binding.Remove(r); };
      btnColor.Click += (_, __) =>
      {
        if (grid.CurrentRow?.DataBoundItem is Rule r)
        {
          using var dlg = new ColorDialog();
          dlg.Color = System.Drawing.ColorTranslator.FromHtml(r.ColorHex);
          if (dlg.ShowDialog() == DialogResult.OK)
          {
            r.ColorHex = ColorTranslator.ToHtml(dlg.Color);
            grid.Refresh();
          }
        }
      };

      grid.DataSource = _binding;

      Controls.Add(chkTabBg);
      Controls.Add(chkTopStripe);
      Controls.Add(chkTitlePrefix);
      Controls.Add(chkRegex);
      Controls.Add(txtPrefix);
      Controls.Add(grid);
      Controls.Add(btnAdd);
      Controls.Add(btnRemove);
      Controls.Add(btnColor);
    }

    public void LoadFrom(TabColoringSettings s)
    {
      chkTabBg.Checked = s.EnableTabBackground;
      chkTopStripe.Checked = s.EnableTopStripe;
      chkTitlePrefix.Checked = s.EnableTitlePrefix;
      txtPrefix.Text = s.TitlePrefix;
      chkRegex.Checked = s.UseRegex;

      _binding.Clear();
      foreach (var r in s.GetRules()) _binding.Add(r);
    }

    public void SaveTo(TabColoringSettings s)
    {
      s.EnableTabBackground = chkTabBg.Checked;
      s.EnableTopStripe = chkTopStripe.Checked;
      s.EnableTitlePrefix = chkTitlePrefix.Checked;
      s.TitlePrefix = txtPrefix.Text;
      s.UseRegex = chkRegex.Checked;
      s.SetRules(_binding);
    }
  }
}
