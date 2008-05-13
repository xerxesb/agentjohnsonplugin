using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AgentJohnson.ValueAnalysis;
using EnvDTE;
using JetBrains.UI.Options;
using JetBrains.VSIntegration.Shell;
using Sitecore.Annotations;
using Sitecore.Diagnostics;

namespace AgentJohnson.Options
{
  /// <summary>
  /// 
  /// </summary>
  [OptionsPage(NAME, "Assertions and Value Analysis", "AgentJohnson.Resources.Assertions.gif",
    ParentId = ImportExportPage.NAME)]
  public partial class ValueAnalysisOptionsPage : UserControl, IOptionsPage
  {
    #region Constants

    /// <summary>
    /// </summary>
    public const string NAME = "AgentJohnson.ValueAnalysisAnnotationsPage";

    private static ValueAnalysisOptionsPage _instance;

    #endregion

    #region Fields

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueAnalysisOptionsPage"/> class.
    /// </summary>
    public ValueAnalysisOptionsPage()
    {
      InitializeComponent();
      _instance = this;

      _DTE dte = VSShell.Instance.ApplicationObject;


      Command command;
      try
      {
        command = dte.Commands.Item("Weigelt.GhostDoc.AddIn.DocumentThis", -1);
      }
      catch
      {
        command = null;
      }

      if (command == null)
      {
        ExecuteGhostDoc.Enabled = false;
      }
    }

    #endregion

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static ValueAnalysisOptionsPage Instance
    {
      get { return _instance; }
    }

    #region Public methods

    /// <summary>
    /// Invoked when OK button in the options dialog is pressed
    /// If the page returns <c>false</c>, the the options dialog won't be closed, and focus
    /// will be put into this page
    /// </summary>
    /// <returns></returns>
    public bool OnOk()
    {
      Commit();

      return true;
    }

    /// <summary>
    /// Check if the settings on the page are consistent, and page could be closed
    /// </summary>
    /// <returns><c>true</c> if page data is consistent</returns>
    public bool ValidatePage()
    {
      return true;
    }

    /// <summary>
    /// Commits this instance.
    /// </summary>
    public void Commit()
    {
      List<Rule> typeConfigurations = ValueAnalysisSettings.Instance.Rules;

      typeConfigurations.Clear();

      foreach (Rule item in Types.Items)
      {
        typeConfigurations.Add(item);
      }

      ValueAnalysisSettings.Instance.AllowNullAttribute = AllowNullAttribute.Text;
      ValueAnalysisSettings.Instance.ExecuteGhostDoc = ExecuteGhostDoc.Checked;
    }

    /// <summary>
    /// Displays this instance.
    /// </summary>
    public void Display()
    {
      List<Rule> rules = ValueAnalysisSettings.Instance.Rules;

      Types.Items.Clear();

      foreach (Rule rule in rules)
      {
        Types.Items.Add(rule.Clone());
      }

      if (Types.Items.Count > 0)
      {
        Types.SetSelected(0, true);
      }

      AllowNullAttribute.Text = ValueAnalysisSettings.Instance.AllowNullAttribute;
      ExecuteGhostDoc.Checked = ValueAnalysisSettings.Instance.ExecuteGhostDoc;
    }

    /// <summary>
    /// Invoked when this page is selected/unselected in the tree
    /// </summary>
    /// <param name="activated">true, when page is selected; false, when page is unselected</param>
    public void OnActivated(bool activated)
    {
    }

    #endregion

    #region Protected methods

    ///<summary>
    ///Raises the <see cref="E:System.Windows.Forms.UserControl.Load" /> event.
    ///</summary>
    ///
    ///<param name="e">An <see cref="T:System.EventArgs" /> that contains the event data. </param>
    protected override void OnLoad([NotNull] EventArgs e)
    {
      Assert.ArgumentNotNull(e, "e");

      base.OnLoad(e);

      Display();
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Handles the Button1_ click event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void Add_Click(object sender, EventArgs e)
    {
      var page = new ValueAnalysisDetailsPage();

      var rule = new Rule();

      page.Display(rule);

      if (page.ShowDialog() != DialogResult.OK)
      {
        return;
      }

      int index = Types.Items.Add(rule);

      Types.SetSelected(index, true);
    }

    /// <summary>
    /// Handles the Button2_ click event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void Remove_Click(object sender, EventArgs e)
    {
      int selectedIndex = Types.SelectedIndex;
      if (selectedIndex < 0)
      {
        return;
      }

      if (
        MessageBox.Show("Are you sure you want to remove this rule?", "Remove", MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Asterisk) != DialogResult.OK)
      {
        return;
      }

      Types.Items.RemoveAt(selectedIndex);

      if (selectedIndex > 0)
      {
        selectedIndex--;
      }

      if (selectedIndex > 0)
      {
        Types.SetSelected(selectedIndex, true);
      }
    }

    /// <summary>
    /// Handles the Edit_ click event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void Edit_Click(object sender, EventArgs e)
    {
      int selectedIndex = Types.SelectedIndex;
      if (selectedIndex < 0)
      {
        return;
      }

      var rule = Types.Items[selectedIndex] as Rule;

      var page = new ValueAnalysisDetailsPage();

      page.Display(rule);

      if (page.ShowDialog() != DialogResult.OK)
      {
        return;
      }

      Types.Items[selectedIndex] = rule;
    }

    #endregion

    #region IOptionsPage Members

    /// <summary>
    /// Control to be shown as page
    /// </summary>
    /// <value></value>
    [NotNull]
    public Control Control
    {
      get { return this; }
    }

    /// <summary>
    /// Gets the ID of this option page.
    /// <see cref="T:JetBrains.UI.Options.IOptionsDialog"/> or <see cref="T:JetBrains.UI.Options.OptionsPageDescriptor"/> could be used to retrieve the <see cref="T:JetBrains.UI.Options.OptionsManager"/> out of it.
    /// </summary>
    /// <value></value>
    [NotNull]
    public string Id
    {
      get { return NAME; }
    }

    #endregion
  }
}