// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueAnalysisDetailsPage.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The value analysis details page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Options
{
  using System;
  using System.Windows.Forms;
  using ValueAnalysis;

  /// <summary>
  /// The value analysis details page.
  /// </summary>
  public partial class ValueAnalysisDetailsPage : Form
  {
    #region Constants and Fields

    /// <summary>
    /// The _rule.
    /// </summary>
    private Rule _rule;

    /// <summary>
    /// The _updating.
    /// </summary>
    private bool _updating;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueAnalysisDetailsPage"/> class.
    /// </summary>
    public ValueAnalysisDetailsPage()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Commits the specified item.
    /// </summary>
    /// <param name="item">
    /// The item.
    /// </param>
    public void Commit(Rule item)
    {
      item.TypeName = this.TypeName.Text;
      item.NotNull = this.NotNull.Checked;
      item.CanBeNull = this.CanBeNull.Checked;
      item.PublicParameterAssertion = this.PublicParameterAssertion.Text;
      item.NonPublicParameterAssertion = this.NonPublicParameterAssertion.Text;
      item.ReturnAssertion = this.ReturnAssertion.Text;

      item.ValueAssertions.Clear();

      var rows = this.ValueAssertions.Rows;

      foreach (DataGridViewRow row in rows)
      {
        var assertion = row.Cells[0].Value as string;

        if (assertion != null)
        {
          item.ValueAssertions.Add(assertion);
        }
      }
    }

    /// <summary>
    /// Displays the specified item.
    /// </summary>
    /// <param name="rule">
    /// The item.
    /// </param>
    public void Display(Rule rule)
    {
      this._rule = rule;

      this.TypeName.Text = rule.TypeName;
      this.NotNull.Checked = rule.NotNull;
      this.CanBeNull.Checked = rule.CanBeNull;
      this.PublicParameterAssertion.Text = rule.PublicParameterAssertion;
      this.NonPublicParameterAssertion.Text = rule.NonPublicParameterAssertion;
      this.ReturnAssertion.Text = rule.ReturnAssertion;

      var rows = this.ValueAssertions.Rows;

      rows.Clear();

      foreach (var key in rule.ValueAssertions)
      {
        rows.Add(new[]
        {
          key
        });
      }

      this.EnableButtons();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handles the CheckedChanged event of the CanBeNull control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data.
    /// </param>
    private void CanBeNull_CheckedChanged(object sender, EventArgs e)
    {
      if (this._updating)
      {
        return;
      }

      if (this.NotNull.Checked)
      {
        this._updating = true;
        this.CanBeNull.Checked = true;
        this.NotNull.Checked = false;
        this._updating = false;
      }
    }

    /// <summary>
    /// Enables the buttons.
    /// </summary>
    private void EnableButtons()
    {
      this.button1.Enabled = !string.IsNullOrEmpty(this.TypeName.Text);
    }

    /// <summary>
    /// Handles the CheckedChanged event of the NotNull control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data.
    /// </param>
    private void NotNull_CheckedChanged(object sender, EventArgs e)
    {
      if (this._updating)
      {
        return;
      }

      if (this.CanBeNull.Checked)
      {
        this._updating = true;
        this.NotNull.Checked = true;
        this.CanBeNull.Checked = false;
        this._updating = false;
      }
    }

    /// <summary>
    /// Handles the O k_ click event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data.
    /// </param>
    private void OK_Click(object sender, EventArgs e)
    {
      this.Commit(this._rule);

      this.DialogResult = DialogResult.OK;

      this.Close();
    }

    /// <summary>
    /// Handles the TextChanged event of the TypeName control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data.
    /// </param>
    private void TypeName_TextChanged(object sender, EventArgs e)
    {
      this.EnableButtons();
    }

    #endregion
  }
}