using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using AgentJohnson.ValueAnalysis;
using JetBrains.UI.Options;

namespace AgentJohnson.Options {
  [OptionsPage(
    "ReSharper.ValueAnalysisAnnotations",
    "Value Analysis Annotations",
    null,
    ParentId = "CodeInspection")]
  public partial class ValueAnalysisOptionsPage : UserControl, IOptionsPage {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueAnalysisOptionsPage"/> class.
    /// </summary>
    public ValueAnalysisOptionsPage() {
      InitializeComponent();
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Control to be shown as page
    /// </summary>
    /// <value></value>
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
    public string Id {
      get {
        return "ReSharper.ValueAnalysisAnnotations";
      }
    }

    #endregion

    #region Public methods

    ///<summary>
    ///Raises the <see cref="E:System.Windows.Forms.UserControl.Load" /> event.
    ///</summary>
    ///
    ///<param name="e">An <see cref="T:System.EventArgs" /> that contains the event data. </param>
    protected override void OnLoad(EventArgs e) {
      base.OnLoad(e);

      NameValueCollection typeAssertions = ValueAnalysisSettings.Instance.TypeAssertions;

      foreach (string key in typeAssertions) {
        typeAssertionsGridView.Rows.Add(new string[] { key, typeAssertions[key] });
      }

      PublicMethods.Checked = ValueAnalysisSettings.Instance.PublicMethods;
      InternalMethods.Checked = ValueAnalysisSettings.Instance.InternalMethods;
      ProtectedMethods.Checked = ValueAnalysisSettings.Instance.ProtectedMethods;
      PrivateMethods.Checked = ValueAnalysisSettings.Instance.PrivateMethods;

      AllowNullAttribute.Text = ValueAnalysisSettings.Instance.AllowNullAttribute;
    }

    /// <summary>
    /// Invoked when this page is selected/unselected in the tree
    /// </summary>
    /// <param name="activated">true, when page is selected; false, when page is unselected</param>
    public void OnActivated(bool activated) {
    }

    /// <summary>
    /// Check if the settings on the page are consistent, and page could be closed
    /// </summary>
    /// <returns><c>true</c> if page data is consistent</returns>
    public bool ValidatePage() {
      return true;
    }

    /// <summary>
    /// Invoked when OK button in the options dialog is pressed
    /// If the page returns <c>false</c>, the the options dialog won't be closed, and focus
    /// will be put into this page
    /// </summary>
    /// <returns></returns>
    public bool OnOk() {
      NameValueCollection typeAssertions = ValueAnalysisSettings.Instance.TypeAssertions;
      typeAssertions.Clear();

      DataGridViewRowCollection rows = typeAssertionsGridView.Rows;

      foreach(DataGridViewRow row in rows){
        object key = row.Cells[0].Value;
        object value = row.Cells[1].Value;

        if (key != null && value != null){
          typeAssertions.Add(key.ToString(), value.ToString());
        }
      }

      ValueAnalysisSettings.Instance.PublicMethods = PublicMethods.Checked;
      ValueAnalysisSettings.Instance.InternalMethods = InternalMethods.Checked;
      ValueAnalysisSettings.Instance.ProtectedMethods = ProtectedMethods.Checked;
      ValueAnalysisSettings.Instance.PrivateMethods = PrivateMethods.Checked;

      ValueAnalysisSettings.Instance.AllowNullAttribute = AllowNullAttribute.Text;

      return true;
    }

    #endregion
  }
}