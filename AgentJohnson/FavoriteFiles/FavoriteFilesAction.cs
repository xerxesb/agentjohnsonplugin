namespace AgentJohnson.FavoriteFiles
{
  using System.Collections.Generic;
  using System.Drawing;
  using System.IO;
  using System.Windows.Forms;
  using EnvDTE;
  using JetBrains.ActionManagement;
  using JetBrains.CommonControls;
  using JetBrains.DocumentModel;
  using JetBrains.IDE;
  using JetBrains.ProjectModel;
  using JetBrains.UI.PopupMenu;
  using JetBrains.UI.RichText;
  using JetBrains.Util;
  using JetBrains.VSIntegration.Application;

  /// <summary>
  /// Handles Find Text action, see Actions.xml
  /// </summary>
  [ActionHandler("FavoriteFiles")]
  public class FavoriteFilesAction : IActionHandler
  {
    #region Fields

    private FavoriteFilePath _currentFile;
    private ISolution _solution;

    #endregion

    #region Public methods

    ///<summary>
    /// Executes action. Called after Update, that set <see cref="ActionPresentation"/>.Enabled to true.
    ///</summary>
    ///<param name="context"><c>DataContext</c></param>
    ///<param name="nextExecute">delegate to call</param>
    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
      ISolution solution = context.GetData(DataConstants.SOLUTION);
      if (solution == null)
      {
        return;
      }

      Execute(solution, context);
    }

    ///<summary>
    /// Updates action visual presentation. If presentation.Enabled is set to false, Execute
    /// will not be called.
    ///</summary>
    ///<param name="context"><c>DataContext</c></param>
    ///<param name="presentation">presentation to update</param>
    ///<param name="nextUpdate">delegate to call</param>
    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
      return context.CheckAllNotNull(DataConstants.SOLUTION);
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Describes the favorite file.
    /// </summary>
    /// <param name="favoriteFilePath">The favorite file path.</param>
    /// <param name="index">The index.</param>
    /// <returns>The favorite file.</returns>
    private static SimpleMenuItem DescribeFavoriteFile(FavoriteFilePath favoriteFilePath, int index)
    {
      SimpleMenuItem result = new SimpleMenuItem();

      if (favoriteFilePath == null)
      {
        result.Text = "<Error>";
        return result;
      }

      try
      {
        result.Text = Path.GetFileName(favoriteFilePath.Path);
      }
      catch
      {
        result.Text = favoriteFilePath.Path;
      }
      result.Style = MenuItemStyle.Enabled;
      result.ShortcutText = new RichText("(" + favoriteFilePath + ")", TextStyle.FromForeColor(Color.LightGray));

      if (index < 0 || index > 8)
      {
        return result;
      }

      result.Mnemonic = index.ToString();

      return result;
    }

    /// <summary>
    /// Gets the current file.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="dataContext">The data context.</param>
    /// <returns>The current file.</returns>
    private static FavoriteFilePath GetCurrentFile(ISolution solution, IDataContext dataContext)
    {
      IDocument document = dataContext.GetData(DataConstants.DOCUMENT);
      if (document == null)
      {
        return null;
      }

      IProjectFile projectFile = DocumentManager.GetInstance(solution).GetProjectFile(document);
      if (projectFile == null)
      {
        return null;
      }

      FavoriteFilePath result = new FavoriteFilePath();

      FileSystemPath path = projectFile.Location;

      IProject project = projectFile.GetProject();

      // miscellaneous files
      if (!project.IsWritable && project.LanguageType == ProjectFileType.UNKNOWN && project.Location.IsEmpty)
      {
        project = null;
      }

      if (project != null)
      {
        result.ProjectName = project.Name;
        path = projectFile.Location.ConvertToRelativePath(project.Location);
      }

      result.Path = path.ToString();

      return result;
    }

    /// <summary>
    /// Organizes this instance.
    /// </summary>
    private static void Organize()
    {
      using (OrganizeDialog organizeDialog = new OrganizeDialog())
      {
        organizeDialog.ShowDialog();
      }
    }

    /// <summary>
    /// Adds the current file.
    /// </summary>
    private void AddCurrentFile()
    {
      if (this._currentFile == null)
      {
        return;
      }

      List<FavoriteFilePath> favoriteFiles = FavoriteFilesSettings.Instance.FavoriteFiles;

      for (int n = favoriteFiles.Count - 1; n >= 0; n--)
      {
        FavoriteFilePath existingFavoriteFile = favoriteFiles[n];

        if (existingFavoriteFile.Path == this._currentFile.Path && existingFavoriteFile.ProjectName == this._currentFile.ProjectName)
        {
          favoriteFiles.RemoveAt(n);
        }
      }

      favoriteFiles.Add(this._currentFile);
    }

    /// <summary>
    /// Describes the add menu item.
    /// </summary>
    /// <returns>The add menu item.</returns>
    private SimpleMenuItem DescribeAddMenuItem()
    {
      SimpleMenuItem result = new SimpleMenuItem
      {
        Text = new RichText("Add Current File")
      };

      result.Clicked += delegate { this.AddCurrentFile(); };

      if (this._currentFile != null)
      {
        result.Style = MenuItemStyle.Enabled;
      }

      return result;
    }

    /// <summary>
    /// Describes the more menu item.
    /// </summary>
    /// <returns>The more menu item.</returns>
    private SimpleMenuItem DescribeOrganizeMenuItem()
    {
      SimpleMenuItem result = new SimpleMenuItem
      {
        Text = new RichText("Organize...")
      };

      result.Clicked += delegate { Organize(); };

      if (this._currentFile != null)
      {
        result.Style = MenuItemStyle.Enabled;
      }

      return result;
    }

    /// <summary>
    /// Executes action. Called after Update, that set <c>ActionPresentation</c>.Enabled to true.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    private void Execute(ISolution solution, IDataContext context)
    {
      this._solution = solution;
      this._currentFile = GetCurrentFile(this._solution, context);

      List<SimpleMenuItem> items = new List<SimpleMenuItem>();

      List<FavoriteFilePath> files = FavoriteFilesSettings.Instance.FavoriteFiles;

      int index = 0;

      foreach (FavoriteFilePath favoriteFilePath in files)
      {
        FavoriteFilePath path = favoriteFilePath;

        if (string.IsNullOrEmpty(favoriteFilePath.ProjectName))
        {
          SimpleMenuItem item = DescribeFavoriteFile(favoriteFilePath, index);

          item.Clicked += delegate { this.menu_ItemClicked(path); };

          items.Add(item);

          index++;

          continue;
        }

        IProject project = solution.GetProject(favoriteFilePath.ProjectName);

        if (project != null)
        {
          SimpleMenuItem item = DescribeFavoriteFile(favoriteFilePath, index);

          item.Clicked += delegate { this.menu_ItemClicked(path); };

          items.Add(item);

          index++;

          continue;
        }
      }

      if (items.Count > 0)
      {
        items.Add(SimpleMenuItem.CreateSeparator());
      }

      items.Add(this.DescribeAddMenuItem());
      items.Add(this.DescribeOrganizeMenuItem());

      JetPopupMenu menu = new JetPopupMenu();

      menu.Caption.Value = WindowlessControl.Create("Favorite Files");
      menu.SetItems(items.ToArray());
      menu.KeyboardAcceleration.SetValue(KeyboardAccelerationFlags.Mnemonics);

      menu.Show();
    }

    /// <summary>
    /// Opens the favorite file.
    /// </summary>
    /// <param name="favoriteFilePath">The favorite file path.</param>
    private void OpenFavoriteFile(FavoriteFilePath favoriteFilePath)
    {
      FileSystemPath path = new FileSystemPath(favoriteFilePath.Path);

      if (string.IsNullOrEmpty(favoriteFilePath.ProjectName))
      {
        _DTE dte = VSShell.Instance.ApplicationObject;
        dte.ItemOperations.OpenFile(favoriteFilePath.Path, Constants.vsViewKindTextView);
        return;
      }

      if (!string.IsNullOrEmpty(favoriteFilePath.ProjectName))
      {
        IProject project = this._solution.GetProject(favoriteFilePath.ProjectName);

        if (project == null)
        {
          return;
        }

        FileSystemPath projectPath = project.Location;

        path = projectPath.Combine(path);
      }

      IList<IProjectItem> location = this._solution.FindProjectItemsByLocation(path);
      if (location == null || location.Length() == 0)
      {
        return;
      }

      IProjectItem projectItem = location[0];

      if (projectItem == null)
      {
        System.Windows.Forms.MessageBox.Show("File not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if (projectItem.Kind != ProjectItemKind.PHYSICAL_FILE)
      {
        System.Windows.Forms.MessageBox.Show("File not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      IProjectFile projectFile = projectItem as IProjectFile;
      if (projectFile == null)
      {
        System.Windows.Forms.MessageBox.Show("File not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if (EditorManager.GetInstance(this._solution).OpenProjectFile(projectFile, true) == null)
      {
        System.Windows.Forms.MessageBox.Show("File not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    #endregion

    #region Events

    /// <summary>
    /// Handles the ItemClicked event of the menu control.
    /// </summary>
    /// <param name="path">The path.</param>
    private void menu_ItemClicked(FavoriteFilePath path)
    {
      if (path.Path == "__add")
      {
        this.AddCurrentFile();
      }
      else if (path.Path == "__more")
      {
        Organize();
      }
      else
      {
        this.OpenFavoriteFile(path);
      }
    }

    #endregion
  }
}