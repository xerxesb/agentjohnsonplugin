namespace AgentJohnson.Options
{
  using System;
  using System.Collections.Generic;
  using System.Windows.Forms;
  using JetBrains.UI.Options;
  using Strings;

  /// <summary>
  /// 
  /// </summary>
  [OptionsPage("AgentJohnson.IntroduceStringConstant", "Introduce String Constant", "AgentJohnson.Resources.StringConstant.gif", ParentId = ImportExportPage.NAME)]
  public partial class IntroduceStringConstantOptionsPage : UserControl, IOptionsPage
  {
    #region Fields

    private static IntroduceStringConstantOptionsPage _instance;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="IntroduceStringConstantOptionsPage"/> class.
    /// </summary>
    public IntroduceStringConstantOptionsPage()
    {
      this.InitializeComponent();
      _instance = this;
    }

    #endregion

    #region Public properties

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

    #endregion

    #region Public methods

    /// <summary>
    /// Commits the specified settings.
    /// </summary>
    public void Commit()
    {
      IntroduceStringConstantSettings settings = IntroduceStringConstantSettings.Instance;

      List<string> classNames = settings.ClassNames;
      classNames.Clear();

      DataGridViewRowCollection rows = this.methodsGridView.Rows;

      foreach (DataGridViewRow row in rows)
      {
        object value = row.Cells[0].Value;

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
      IntroduceStringConstantSettings settings = IntroduceStringConstantSettings.Instance;

      List<string> classNames = settings.ClassNames;

      this.methodsGridView.Rows.Clear();

      foreach (string className in classNames)
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

    ///<summary>
    ///
    ///            Invoked when this page is selected/unselected in the tree.
    ///            
    ///</summary>
    ///
    ///<param name="activated"><c>True</c>, when page is selected; <c>False</c>, when page is unselected.</param>
    public void OnActivated(bool activated)
    {
    }

    /// <summary>
    /// Invoked when OK button in the options dialog is pressed
    /// If the page returns <c>false</c>, the the options dialog won't be closed, and focus
    /// will be put into this page
    /// </summary>
    /// <returns></returns>
    public bool OnOk()
    {
      this.Commit();

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

    #endregion

    #region Protected methods

    ///<summary>
    ///Raises the <see cref="E:System.Windows.Forms.UserControl.Load"></see> event.
    ///</summary>
    ///
    ///<param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data. </param>
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      this.Display();
    }

    #endregion

    #region IOptionsPage Members

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
  }
}