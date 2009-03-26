using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AgentJohnson.SmartGenerate;
using JetBrains.UI.Options;

namespace AgentJohnson.Options {
  /// <summary>
  /// 
  /// </summary>
  [OptionsPage(NAME, "Smart Generate", "AgentJohnson.Resources.SmartGenerate.gif", ParentId = ImportExportPage.NAME, Sequence = 2)]
  public partial class SmartGenerateOptionsPage : UserControl, IOptionsPage, IComparer<SmartGenerateHandlerData> {
    #region Constants

    public const string NAME = "AgentJohnson.SmartGenerateOptionsPage";

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportExportPage"/> class.
    /// </summary>
    public SmartGenerateOptionsPage() {
      InitializeComponent();

      List<SmartGenerateHandlerData> list = new List<SmartGenerateHandlerData>();

      foreach(SmartGenerateHandlerData handler in SmartGenerateManager.Instance.Handlers) {
        list.Add(handler);
      }

      list.Sort(this);

      List<string> disabledHandlers = new List<string>(SmartGenerateSettings.Instance.DisabledActions.Split('|'));

      foreach(SmartGenerateHandlerData handler in list) {
        bool isDisabled = disabledHandlers.Contains(handler.Name);

        Handlers.Items.Add(handler, !isDisabled);
      }

      EnableButtons();
    }

    /// <summary>
    /// Enables the buttons.
    /// </summary>
    void EnableButtons() {
      if(Handlers.SelectedIndex < 0) {
        ActionName.Text = string.Empty;
        ActionDescription.Text = string.Empty;
        return;
      }

      SmartGenerateHandlerData selected = (SmartGenerateHandlerData)Handlers.SelectedItem;

      ActionName.Text = selected.Name;
      ActionDescription.Text = selected.Description;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Invoked when OK button in the options dialog is pressed.
    /// If the page returns <c>false</c>, the the options dialog won't be closed, and focus will be put into this page.
    /// </summary>
    /// <returns></returns>
    public bool OnOk() {
      string disabledHandlers = string.Empty;

      CheckedListBox.CheckedIndexCollection indices = Handlers.CheckedIndices;

      for(int n = 0; n < Handlers.Items.Count; n++) {
        if (indices.Contains(n)) {
          continue;
        }

        SmartGenerateHandlerData handler = Handlers.Items[n] as SmartGenerateHandlerData;
        if (handler == null) {
          continue;
        }

        if (!string.IsNullOrEmpty(disabledHandlers)) {
          disabledHandlers += "|";
        }

        disabledHandlers += handler.Name;
      }

      SmartGenerateSettings.Instance.DisabledActions = disabledHandlers;

      return true;
    }

    /// <summary>
    /// Check if the settings on the page are consistent, and page could be closed.
    /// </summary>
    /// <returns><c>true</c> if page data is consistent.</returns>
    public bool ValidatePage() {
      return true;
    }

    #endregion

    #region IComparer<SmartGenerateHandler> Members

    /// <summary>
    /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>
    /// Value Condition Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
    /// </returns>
    int IComparer<SmartGenerateHandlerData>.Compare(SmartGenerateHandlerData x, SmartGenerateHandlerData y) {
      return string.Compare(x.Name, y.Name);
    }

    #endregion

    #region IOptionsPage Members

    /// <summary>
    /// Control to be shown as page.
    /// May be <c>Null</c> if the page does not have any UI.
    /// </summary>
    /// <value></value>
    Control IOptionsPage.Control {
      get {
        return this;
      }
    }

    /// <summary>
    /// Gets the ID of this option page.
    /// <see cref="T:JetBrains.UI.Options.IOptionsDialog"/> or <see cref="T:JetBrains.UI.Options.OptionsPageDescriptor"/> could be used to retrieve the <see cref="T:JetBrains.UI.Options.OptionsManager"/> out of it.
    /// </summary>
    /// <value></value>
    string IOptionsPage.Id {
      get {
        return NAME;
      }
    }

    #endregion

    /// <summary>
    /// Handles the SelectedIndexChanged event of the Handlers control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void Handlers_SelectedIndexChanged(object sender, EventArgs e) {
      EnableButtons();
    }
  }
}