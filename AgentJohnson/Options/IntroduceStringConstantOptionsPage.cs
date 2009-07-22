// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntroduceStringConstantOptionsPage.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The introduce string constant options page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Options
{
  using System;
  using System.Windows.Forms;
  using Strings;
  using JetBrains.UI.Options;

  /// <summary>
  /// The introduce string constant options page.
  /// </summary>
  [OptionsPage("AgentJohnson.IntroduceStringConstant", "Introduce String Constant", "AgentJohnson.Resources.StringConstant.gif", ParentId = ImportExportPage.NAME)]
  public partial class IntroduceStringConstantOptionsPage : UserControl, IOptionsPage
  {
    #region Constants and Fields

    /// <summary>
    /// The _instance.
    /// </summary>
    private static IntroduceStringConstantOptionsPage _instance;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="IntroduceStringConstantOptionsPage"/> class.
    /// </summary>
    public IntroduceStringConstantOptionsPage()
    {
      this.InitializeComponent();
      _instance = this;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the instance.
    /// </summary>
    /// <value>The instance.</value>
    public static IntroduceStringConstantOptionsPage Instance
    {
      get
      {
        return _instance;
      }
    }

    /// <summary>
    /// Control to be shown as page
    /// </summary>
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
        return "AgentJohnson.IntroduceStringConstant";
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Commits the specified settings.
    /// </summary>
    public void Commit()
    {
      var settings = IntroduceStringConstantSettings.Instance;

      var classNames = settings.ClassNames;
      classNames.Clear();

      var rows = this.methodsGridView.Rows;

      foreach (DataGridViewRow row in rows)
      {
        var value = row.Cells[0].Value;

        if (value != null)
        {
          classNames.Add(value.ToString());
        }
      }

      settings.ReplaceSpacesMode = this.rbReplaceWithUnderscore.Checked ? 0 : 1;

      settings.GenerateXmlComment = this.cbGenerateXmlComment.Checked;
    }

    /// <summary>
    /// Displays this instance.
    /// </summary>
    public void Display()
    {
      var settings = IntroduceStringConstantSettings.Instance;

      var classNames = settings.ClassNames;

      this.methodsGridView.Rows.Clear();

      foreach (var className in classNames)
      {
        this.methodsGridView.Rows.Add(new[]
        {
          className
        });
      }

      this.rbReplaceWithUnderscore.Checked = settings.ReplaceSpacesMode == 0;
      this.rbReplaceWithNothing.Checked = settings.ReplaceSpacesMode == 1;

      this.cbGenerateXmlComment.Checked = settings.GenerateXmlComment;
    }

    /// <summary>
    /// Invoked when this page is selected/unselected in the tree.
    /// </summary>
    /// <param name="activated">
    /// <c>True</c>, when page is selected; <c>False</c>, when page is unselected.
    /// </param>
    public void OnActivated(bool activated)
    {
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
    /// The on ok.
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
    /// Raises the <see cref="E:System.Windows.Forms.UserControl.Load"></see> event.
    /// </summary>
    /// <param name="e">
    /// An <see cref="T:System.EventArgs"></see> that contains the event data. 
    /// </param>
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      this.Display();
    }

    #endregion
  }
}