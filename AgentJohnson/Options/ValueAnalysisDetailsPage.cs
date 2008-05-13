using System;
using System.Windows.Forms;
using AgentJohnson.ValueAnalysis;

namespace AgentJohnson.Options {
  /// <summary>
  /// 
  /// </summary>
  public partial class ValueAnalysisDetailsPage : Form {
    #region Fields

    Rule _rule;
    bool _updating;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueAnalysisDetailsPage"/> class.
    /// </summary>
    public ValueAnalysisDetailsPage() {
      InitializeComponent();
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Commits the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    public void Commit(Rule item) {
      item.TypeName = TypeName.Text;
      item.NotNull = NotNull.Checked;
      item.CanBeNull = CanBeNull.Checked;
      item.PublicParameterAssertion = PublicParameterAssertion.Text;
      item.NonPublicParameterAssertion = NonPublicParameterAssertion.Text;
      item.ReturnAssertion = ReturnAssertion.Text;

      item.ValueAssertions.Clear();

      DataGridViewRowCollection rows = ValueAssertions.Rows;

      foreach(DataGridViewRow row in rows) {
        var assertion = row.Cells[0].Value as string;

        if(assertion != null) {
          item.ValueAssertions.Add(assertion);
        }
      }
    }

    /// <summary>
    /// Displays the specified item.
    /// </summary>
    /// <param name="rule">The item.</param>
    public void Display(Rule rule) {
      _rule = rule;

      TypeName.Text = rule.TypeName;
      NotNull.Checked = rule.NotNull;
      CanBeNull.Checked = rule.CanBeNull;
      PublicParameterAssertion.Text = rule.PublicParameterAssertion;
      NonPublicParameterAssertion.Text = rule.NonPublicParameterAssertion;
      ReturnAssertion.Text = rule.ReturnAssertion;

      DataGridViewRowCollection rows = ValueAssertions.Rows;

      rows.Clear();

      foreach(string key in rule.ValueAssertions) {
        rows.Add(new[] {key});
      }

      EnableButtons();
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Handles the CheckedChanged event of the CanBeNull control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void CanBeNull_CheckedChanged(object sender, EventArgs e) {
      if(_updating) {
        return;
      }

      if(NotNull.Checked) {
        _updating = true;
        CanBeNull.Checked = true;
        NotNull.Checked = false;
        _updating = false;
      }
    }

    /// <summary>
    /// Enables the buttons.
    /// </summary>
    void EnableButtons() {
      button1.Enabled = !string.IsNullOrEmpty(TypeName.Text);
    }

    /// <summary>
    /// Handles the CheckedChanged event of the NotNull control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void NotNull_CheckedChanged(object sender, EventArgs e) {
      if(_updating) {
        return;
      }

      if(CanBeNull.Checked) {
        _updating = true;
        NotNull.Checked = true;
        CanBeNull.Checked = false;
        _updating = false;
      }
    }

    /// <summary>
    /// Handles the O k_ click event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void OK_Click(object sender, EventArgs e) {
      Commit(_rule);

      DialogResult = DialogResult.OK;

      Close();
    }

    /// <summary>
    /// Handles the TextChanged event of the TypeName control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void TypeName_TextChanged(object sender, EventArgs e) {
      EnableButtons();
    }

    #endregion
  }
}