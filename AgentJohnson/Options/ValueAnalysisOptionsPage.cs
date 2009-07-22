// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueAnalysisOptionsPage.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The value analysis options page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Options
{
  using System;
  using System.Windows.Forms;
  using ValueAnalysis;
  using EnvDTE;
  using JetBrains.UI.Options;
  using JetBrains.VSIntegration.Application;

  /// <summary>
  /// The value analysis options page.
  /// </summary>
  [OptionsPage(PageName, "Assertions and Value Analysis", "AgentJohnson.Resources.Assertions.gif",
    ParentId = ImportExportPage.NAME)]
  public partial class ValueAnalysisOptionsPage : UserControl, IOptionsPage
  {
    #region Constants and Fields

    /// <summary>
    /// The page name.
    /// </summary>
    private const string PageName = "AgentJohnson.ValueAnalysisAnnotationsPage";

    /// <summary>
    /// The _instance.
    /// </summary>
    private static ValueAnalysisOptionsPage instance;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueAnalysisOptionsPage"/> class.
    /// </summary>
    public ValueAnalysisOptionsPage()
    {
      this.InitializeComponent();
      instance = this;

      _DTE dte = VSShell.Instance.ApplicationObject;

      Command command;
      try
      {
        command = dte.Commands.Item("Tools.SubMain.GhostDoc.DocumentThis", -1);
      }
      catch
      {
        command = null;
      }

      if (command == null)
      {
        this.ExecuteGhostDoc.Enabled = false;
      }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static ValueAnalysisOptionsPage Instance
    {
      get
      {
        return instance;
      }
    }

    /// <summary>
    /// Gets the control to be shown as page.
    /// </summary>
    /// <remarks>May be <c>Null</c> if the page does not have any UI.</remarks>
    /// <value></value>
    public Control Control
    {
      get
      {
        return this;
      }
    }

    /// <summary>
    /// Gets the ID of this option page.
    /// <see cref="T:JetBrains.UI.Options.IOptionsDialog"/> or <see cref="T:JetBrains.UI.Options.OptionsPageDescriptor"/> could be used to retrieve the <see cref="T:JetBrains.UI.Options.OptionsManager"/> out of it.
    /// </summary>
    /// <value></value>
    public string Id
    {
      get
      {
        return PageName;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Commits this instance.
    /// </summary>
    public void Commit()
    {
      var typeConfigurations = ValueAnalysisSettings.Instance.Rules;

      typeConfigurations.Clear();

      foreach (Rule item in this.Types.Items)
      {
        typeConfigurations.Add(item);
      }

      ValueAnalysisSettings.Instance.AllowNullAttribute = this.AllowNullAttribute.Text;
      ValueAnalysisSettings.Instance.ExecuteGhostDoc = this.ExecuteGhostDoc.Checked;
    }

    /// <summary>
    /// Displays this instance.
    /// </summary>
    public void Display()
    {
      var rules = ValueAnalysisSettings.Instance.Rules;

      this.Types.Items.Clear();

      foreach (var rule in rules)
      {
        this.Types.Items.Add(rule.Clone());
      }

      if (this.Types.Items.Count > 0)
      {
        this.Types.SetSelected(0, true);
      }

      this.AllowNullAttribute.Text = ValueAnalysisSettings.Instance.AllowNullAttribute;
      this.ExecuteGhostDoc.Checked = ValueAnalysisSettings.Instance.ExecuteGhostDoc;
    }

    #endregion

    #region Implemented Interfaces

    #region IOptionsPage

    /// <summary>
    /// Invoked when OK button in the options dialog is pressed
    /// If the page returns <c>false</c>, the the options dialog won't be closed, and focus
    /// will be put into this page
    /// </summary>
    /// <returns>
    /// <c>True</c>, if OK.
    /// </returns>
    public bool OnOk()
    {
      this.Commit();

      return true;
    }

    /// <summary>
    /// Check if the settings on the page are consistent, and page could be closed
    /// </summary>
    /// <returns>
    /// <c>true</c> if page data is consistent
    /// </returns>
    public bool ValidatePage()
    {
      return true;
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.UserControl.Load"/> event.
    /// </summary>
    /// <param name="e">
    /// An <see cref="T:System.EventArgs"/> that contains the event data. 
    /// </param>
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      this.Display();
    }

    /// <summary>
    /// Handles the Button1_ click event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data.
    /// </param>
    private void Add_Click(object sender, EventArgs e)
    {
      var page = new ValueAnalysisDetailsPage();

      var rule = new Rule();

      page.Display(rule);

      if (page.ShowDialog() != DialogResult.OK)
      {
        return;
      }

      var index = this.Types.Items.Add(rule);

      this.Types.SetSelected(index, true);
    }

    /// <summary>
    /// Handles the Edit_ click event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data.
    /// </param>
    private void Edit_Click(object sender, EventArgs e)
    {
      var selectedIndex = this.Types.SelectedIndex;
      if (selectedIndex < 0)
      {
        return;
      }

      var rule = this.Types.Items[selectedIndex] as Rule;

      var page = new ValueAnalysisDetailsPage();

      page.Display(rule);

      if (page.ShowDialog() != DialogResult.OK)
      {
        return;
      }

      this.Types.Items[selectedIndex] = rule;
    }

    /// <summary>
    /// Handles the Button2_ click event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data.
    /// </param>
    private void Remove_Click(object sender, EventArgs e)
    {
      var selectedIndex = this.Types.SelectedIndex;
      if (selectedIndex < 0)
      {
        return;
      }

      if (
        MessageBox.Show("Are you sure you want to remove this rule?", "Remove", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) != DialogResult.OK)
      {
        return;
      }

      this.Types.Items.RemoveAt(selectedIndex);

      if (selectedIndex > 0)
      {
        selectedIndex--;
      }

      if (selectedIndex > 0)
      {
        this.Types.SetSelected(selectedIndex, true);
      }
    }

    #endregion
  }
}