using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AgentJohnson.ValueAnalysis;
using JetBrains.UI.Options;
using Sitecore.Annotations;
using Sitecore.Diagnostics;

namespace AgentJohnson.Options {
  /// <summary>
  /// 
  /// </summary>
  [OptionsPage(NAME, "Assertions and Value Analysis", "AgentJohnson.Resources.Assertions.gif", ParentId = ImportExportPage.NAME)]
  public partial class ValueAnalysisOptionsPage : UserControl, IOptionsPage {
    #region Constants

    /// <summary>
    /// </summary>
    public const string NAME = "AgentJohnson.ValueAnalysisAnnotationsPage";

    static ValueAnalysisOptionsPage _instance;

    #endregion

    #region Fields

    int _selectedIndex = -1;
    bool _updating;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueAnalysisOptionsPage"/> class.
    /// </summary>
    public ValueAnalysisOptionsPage() {
      InitializeComponent();
      _instance = this;
    }

    #endregion

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static ValueAnalysisOptionsPage Instance {
      get {
        return _instance;
      }
    }

    #region Public methods

    /// <summary>
    /// Commits this instance.
    /// </summary>
    public void Commit() {
      if(_selectedIndex >= 0) {
        Commit(Types.Items[_selectedIndex] as Rule);
      }

      List<Rule> typeConfigurations = ValueAnalysisSettings.Instance.Rules;

      typeConfigurations.Clear();

      foreach(Rule item in Types.Items) {
        typeConfigurations.Add(item);
      }

      ValueAnalysisSettings.Instance.AllowNullAttribute = AllowNullAttribute.Text;
    }

    /// <summary>
    /// Displays this instance.
    /// </summary>
    public void Display() {
      List<Rule> configurations = ValueAnalysisSettings.Instance.Rules;

      _selectedIndex = -1;

      Types.Items.Clear();

      foreach(Rule configuration in configurations) {
        Types.Items.Add(configuration.Clone());
      }

      if(Types.Items.Count > 0) {
        Types.SetSelected(0, true);
      }

      AllowNullAttribute.Text = ValueAnalysisSettings.Instance.AllowNullAttribute;
    }

    /// <summary>
    /// Invoked when this page is selected/unselected in the tree
    /// </summary>
    /// <param name="activated">true, when page is selected; false, when page is unselected</param>
    public void OnActivated(bool activated) {
    }

    /// <summary>
    /// Invoked when OK button in the options dialog is pressed
    /// If the page returns <c>false</c>, the the options dialog won't be closed, and focus
    /// will be put into this page
    /// </summary>
    /// <returns></returns>
    public bool OnOk() {
      Commit();

      return true;
    }

    /// <summary>
    /// Check if the settings on the page are consistent, and page could be closed
    /// </summary>
    /// <returns><c>true</c> if page data is consistent</returns>
    public bool ValidatePage() {
      return true;
    }

    #endregion

    #region Protected methods

    ///<summary>
    ///Raises the <see cref="E:System.Windows.Forms.UserControl.Load" /> event.
    ///</summary>
    ///
    ///<param name="e">An <see cref="T:System.EventArgs" /> that contains the event data. </param>
    protected override void OnLoad([NotNull] EventArgs e) {
      Assert.ArgumentNotNull(e, "e");

      base.OnLoad(e);

      Types.SelectedIndexChanged += new EventHandler(Types_SelectedIndexChanged);

      Display();
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Handles the Button1_ click event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void Add_Click(object sender, EventArgs e) {
      Rule rule = new Rule();

      rule.TypeName = "System.Object";

      Types.BeginUpdate();

      int index = Types.Items.Add(rule);

      Types.EndUpdate();

      Types.SetSelected(index, true);
    }

    /// <summary>
    /// Handles the CheckedChanged event of the CanBeNull control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void CanBeNull_CheckedChanged(object sender, EventArgs e) {
      if (_updating) {
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
    /// Clears this instance.
    /// </summary>
    void Clear() {
      TypeName.Text = string.Empty;
      NotNull.Checked = false;
      CanBeNull.Checked = false;
      PublicParameterAssertion.Text = string.Empty;
      NonPublicParameterAssertion.Text = string.Empty;
      ReturnAssertion.Text = string.Empty;

      ValueAssertions.Rows.Clear();
    }

    /// <summary>
    /// Commits the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    void Commit(Rule item) {
      item.TypeName = TypeName.Text;
      item.NotNull = NotNull.Checked;
      item.CanBeNull = CanBeNull.Checked;
      item.PublicParameterAssertion = PublicParameterAssertion.Text;
      item.NonPublicParameterAssertion = NonPublicParameterAssertion.Text;
      item.ReturnAssertion = ReturnAssertion.Text;

      item.ValueAssertions.Clear();

      DataGridViewRowCollection rows = ValueAssertions.Rows;

      foreach(DataGridViewRow row in rows) {
        string assertion = row.Cells[0].Value as string;

        if(assertion != null) {
          item.ValueAssertions.Add(assertion.ToString());
        }
      }
    }

    /// <summary>
    /// Displays the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    void Display(Rule item) {
      TypeName.Text = item.TypeName;
      NotNull.Checked = item.NotNull;
      CanBeNull.Checked = item.CanBeNull;
      PublicParameterAssertion.Text = item.PublicParameterAssertion;
      NonPublicParameterAssertion.Text = item.NonPublicParameterAssertion;
      ReturnAssertion.Text = item.ReturnAssertion;

      DataGridViewRowCollection rows = ValueAssertions.Rows;

      rows.Clear();

      foreach(string key in item.ValueAssertions) {
        rows.Add(new string[] {key});
      }
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
    /// Handles the Button2_ click event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void Remove_Click(object sender, EventArgs e) {
      if(Types.SelectedItem == null) {
        return;
      }

      int index = Types.SelectedIndex;
      _selectedIndex = -1;

      Types.SelectedIndex = -1;

      Types.BeginUpdate();

      Types.Items.RemoveAt(index);

      Types.EndUpdate();
    }

    /// <summary>
    /// Handles the TextChanged event of the TypeName control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void TypeName_TextChanged(object sender, EventArgs e) {
      Rule configuration = Types.SelectedItem as Rule;
      if(configuration == null) {
        return;
      }

      configuration.TypeName = TypeName.Text;
    }

    /// <summary>
    /// Handles the SelectedIndexChanged event of the Types control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void Types_SelectedIndexChanged(object sender, EventArgs e) {
      if(_selectedIndex >= 0) {
        Commit(Types.Items[_selectedIndex] as Rule);
      }

      _selectedIndex = Types.SelectedIndex;

      Rule item = Types.SelectedItem as Rule;
      if(item != null) {
        Display(item);
      }
      else {
        Clear();
      }
    }

    #endregion

    #region IOptionsPage Members

    /// <summary>
    /// Control to be shown as page
    /// </summary>
    /// <value></value>
    [NotNull]
    public Control Control {
      get {
        return this;
      }
    }

    /// <summary>
    /// Gets the ID of this option page.
    /// <see cref="T:JetBrains.UI.Options.IOptionsDialog"/> or <see cref="T:JetBrains.UI.Options.OptionsPageDescriptor"/> could be used to retrieve the <see cref="T:JetBrains.UI.Options.OptionsManager"/> out of it.
    /// </summary>
    /// <value></value>
    [NotNull]
    public string Id {
      get {
        return NAME;
      }
    }

    #endregion
  }
}