// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmartGenerateOptionsPage.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The smart generate options page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Options
{
  using System;
  using System.Collections.Generic;
  using System.Windows.Forms;
  using SmartGenerate;
  using JetBrains.UI.Options;

  /// <summary>
  /// The smart generate options page.
  /// </summary>
  [OptionsPage(NAME, "Smart Generate", "AgentJohnson.Resources.SmartGenerate.gif", ParentId = ImportExportPage.NAME, Sequence = 2)]
  public partial class SmartGenerateOptionsPage : UserControl, IOptionsPage, IComparer<SmartGenerateHandlerData>
  {
    #region Constants and Fields

    /// <summary>
    /// The name.
    /// </summary>
    public const string NAME = "AgentJohnson.SmartGenerateOptionsPage";

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SmartGenerateOptionsPage"/> class. 
    /// Initializes a new instance of the <see cref="ImportExportPage"/> class.
    /// </summary>
    public SmartGenerateOptionsPage()
    {
      this.InitializeComponent();

      var list = new List<SmartGenerateHandlerData>();

      foreach (var handler in SmartGenerateManager.Instance.Handlers)
      {
        list.Add(handler);
      }

      list.Sort(this);

      var disabledHandlers = new List<string>(SmartGenerateSettings.Instance.DisabledActions.Split('|'));

      foreach (var handler in list)
      {
        var isDisabled = disabledHandlers.Contains(handler.Name);

        this.Handlers.Items.Add(handler, !isDisabled);
      }

      this.EnableButtons();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Control to be shown as page.
    /// May be <c>Null</c> if the page does not have any UI.
    /// </summary>
    /// <value></value>
    Control IOptionsPage.Control
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
    string IOptionsPage.Id
    {
      get
      {
        return NAME;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IComparer<SmartGenerateHandlerData>

    /// <summary>
    /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
    /// </summary>
    /// <param name="x">
    /// The first object to compare.
    /// </param>
    /// <param name="y">
    /// The second object to compare.
    /// </param>
    /// <returns>
    /// Value Condition Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
    /// </returns>
    int IComparer<SmartGenerateHandlerData>.Compare(SmartGenerateHandlerData x, SmartGenerateHandlerData y)
    {
      return string.Compare(x.Name, y.Name);
    }

    #endregion

    #region IOptionsPage

    /// <summary>
    /// Invoked when OK button in the options dialog is pressed.
    /// If the page returns <c>false</c>, the the options dialog won't be closed, and focus will be put into this page.
    /// </summary>
    /// <returns>
    /// The on ok.
    /// </returns>
    public bool OnOk()
    {
      var disabledHandlers = string.Empty;

      var indices = this.Handlers.CheckedIndices;

      for (var n = 0; n < this.Handlers.Items.Count; n++)
      {
        if (indices.Contains(n))
        {
          continue;
        }

        var handler = this.Handlers.Items[n] as SmartGenerateHandlerData;
        if (handler == null)
        {
          continue;
        }

        if (!string.IsNullOrEmpty(disabledHandlers))
        {
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
    /// <returns>
    /// <c>true</c> if page data is consistent.
    /// </returns>
    public bool ValidatePage()
    {
      return true;
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Enables the buttons.
    /// </summary>
    private void EnableButtons()
    {
      if (this.Handlers.SelectedIndex < 0)
      {
        this.ActionName.Text = string.Empty;
        this.ActionDescription.Text = string.Empty;
        return;
      }

      var selected = (SmartGenerateHandlerData)this.Handlers.SelectedItem;

      this.ActionName.Text = selected.Name;
      this.ActionDescription.Text = selected.Description;
    }

    /// <summary>
    /// Handles the SelectedIndexChanged event of the Handlers control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data.
    /// </param>
    private void Handlers_SelectedIndexChanged(object sender, EventArgs e)
    {
      this.EnableButtons();
    }

    #endregion
  }
}