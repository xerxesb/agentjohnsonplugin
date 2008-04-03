using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AgentJohnson.FavoriteFiles;
using EnvDTE;
using JetBrains.ActionManagement;
using JetBrains.CommonControls;
using JetBrains.DocumentModel;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.UI.PopupMenu;
using JetBrains.UI.RichText;
using JetBrains.Util;
using JetBrains.VSIntegration.Shell;

namespace AgentJohnson.FavoriteFiles {
  /// <summary>
  /// Handles Find Text action, see Actions.xml
  /// </summary>
  [ActionHandler("FavoriteFiles")]
  public class FavoriteFilesAction : IActionHandler {
    #region Fields

    FavoriteFilePath _currentFile;
    ISolution _solution;

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes action. Called after Update, that set <c>ActionPresentation</c>.Enabled to true.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    protected void Execute(ISolution solution, IDataContext context) {
      _solution = solution;
      _currentFile = GetCurrentFile(_solution, context);

      List<SimpleMenuItem> items = new List<SimpleMenuItem>();

      List<FavoriteFilePath> files = FavoriteFilesSettings.Instance.FavoriteFiles;

      int index = 0;

      foreach(FavoriteFilePath favoriteFilePath in files) {
        FavoriteFilePath path = favoriteFilePath;

        if(string.IsNullOrEmpty(favoriteFilePath.ProjectName)) {
          SimpleMenuItem item = DescribeFavoriteFile(favoriteFilePath, index);

          item.Clicked += delegate { menu_ItemClicked(path); };

          items.Add(item);

          index++;

          continue;
        }

        IProject project = solution.GetProject(favoriteFilePath.ProjectName);

        if(project != null) {
          SimpleMenuItem item = DescribeFavoriteFile(favoriteFilePath, index);

          item.Clicked += delegate { menu_ItemClicked(path); };

          items.Add(item);

          index++;

          continue;
        }
      }

      if(items.Count > 0) {
        items.Add(SimpleMenuItem.CreateSeparator());
      }

      items.Add(DescribeAddMenuItem());
      items.Add(DescribeOrganizeMenuItem());

      JetPopupMenu menu = new JetPopupMenu();

      menu.Caption.Value = WindowlessControl.Create("Favorite Files");
      menu.SetItems(items);
      menu.KeyboardAcceleration.SetValue(KeyboardAccelerationFlags.Mnemonics);

      menu.Show();
    }

    #endregion

    #region Events

    /// <summary>
    /// Handles the ItemClicked event of the menu control.
    /// </summary>
    /// <param name="path">The path.</param>
    void menu_ItemClicked(FavoriteFilePath path) {
      if (path.Path == "__add") {
        AddCurrentFile();
      } 
      else if(path.Path == "__more") {
        Organize();
      }
      else {
        OpenFavoriteFile(path);
      }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Organizes this instance.
    /// </summary>
    static void Organize() {
      using(OrganizeDialog organizeDialog = new OrganizeDialog()) {
        organizeDialog.ShowDialog();
      }
    }

    /// <summary>
    /// Adds the current file.
    /// </summary>
    void AddCurrentFile() {
      if(_currentFile == null) {
        return;
      }

      List<FavoriteFilePath> favoriteFiles = FavoriteFilesSettings.Instance.FavoriteFiles;

      for(int n = favoriteFiles.Count - 1; n >= 0; n--) {
        FavoriteFilePath existingFavoriteFile = favoriteFiles[n];

        if(existingFavoriteFile.Path == _currentFile.Path && existingFavoriteFile.ProjectName == _currentFile.ProjectName) {
          favoriteFiles.RemoveAt(n);
        }
      }

      favoriteFiles.Add(_currentFile);
    }

    /// <summary>
    /// Describes the add menu item.
    /// </summary>
    /// <returns>The add menu item.</returns>
    SimpleMenuItem DescribeAddMenuItem() {
      SimpleMenuItem result = new SimpleMenuItem();

      result.Text = new RichText("Add Current File");

      result.Clicked += delegate {
        AddCurrentFile();
      };

      if(_currentFile != null) {
        result.Style = MenuItemStyle.Enabled;
      }

      return result;
    }

    /// <summary>
    /// Describes the favorite file.
    /// </summary>
    /// <param name="favoriteFilePath">The favorite file path.</param>
    /// <param name="index">The index.</param>
    /// <returns>The favorite file.</returns>
    static SimpleMenuItem DescribeFavoriteFile(FavoriteFilePath favoriteFilePath, int index) {
      SimpleMenuItem result = new SimpleMenuItem();

      if(favoriteFilePath == null) {
        result.Text = "<Error>";
        return result;
      }

      try{
        result.Text = Path.GetFileName(favoriteFilePath.Path);
      }
      catch{
        result.Text = favoriteFilePath.Path;
      }
      result.Style = MenuItemStyle.Enabled;
      result.ShortcutText = new RichText("(" + favoriteFilePath + ")", TextStyle.FromForeColor(Color.LightGray));

      if (index < 0 || index > 8){
        return result;
      }

      result.Mnemonic = index.ToString();

      return result;
    }

    /// <summary>
    /// Describes the more menu item.
    /// </summary>
    /// <returns>The more menu item.</returns>
    SimpleMenuItem DescribeOrganizeMenuItem() {
      SimpleMenuItem result = new SimpleMenuItem();

      result.Text = new RichText("Organize...");

      result.Clicked += delegate {
        Organize();
      };

      if(_currentFile != null) {
        result.Style = MenuItemStyle.Enabled;
      }

      return result;
    }

    /// <summary>
    /// Gets the current file.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="dataContext">The data context.</param>
    /// <returns>The current file.</returns>
    static FavoriteFilePath GetCurrentFile(ISolution solution, IDataContext dataContext) {
      IDocument document = dataContext.GetData(DataConstants.DOCUMENT);
      if(document == null) {
        return null;
      }

      IProjectFile projectFile = DocumentManager.GetInstance(solution).GetProjectFile(document);
      if(projectFile == null) {
        return null;
      }

      FavoriteFilePath result = new FavoriteFilePath();

      FileSystemPath path = projectFile.Location;

      IProject project = projectFile.GetProject();

      // miscellaneous files
      if(!project.IsWritable && project.LanguageType == ProjectFileType.UNKNOWN && project.Location.IsEmpty) {
        project = null;
      }

      if(project != null) {
        result.ProjectName = project.Name;
        path = projectFile.Location.ConvertToRelativePath(project.Location);
      }

      result.Path = path.ToString();

      return result;
    }

    /// <summary>
    /// Opens the favorite file.
    /// </summary>
    /// <param name="favoriteFilePath">The favorite file path.</param>
    void OpenFavoriteFile(FavoriteFilePath favoriteFilePath) {
      FileSystemPath path = new FileSystemPath(favoriteFilePath.Path);

      if(string.IsNullOrEmpty(favoriteFilePath.ProjectName)){
        _DTE dte = VSShell.Instance.ApplicationObject;
        dte.ItemOperations.OpenFile(favoriteFilePath.Path, Constants.vsViewKindTextView);
        return;
      }

      if(!string.IsNullOrEmpty(favoriteFilePath.ProjectName)) {
        IProject project = _solution.GetProject(favoriteFilePath.ProjectName);

        if(project == null) {
          return;
        }

        FileSystemPath projectPath = project.Location;

        path = projectPath.Combine(path);
      }

      IProjectItem projectItem = _solution.FindProjectItemByLocation(path);

      if(projectItem == null) {
        MessageBox.Show("File not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if(projectItem.Kind != ProjectItemKind.PHYSICAL_FILE) {
        MessageBox.Show("File not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      IProjectFile projectFile = projectItem as IProjectFile;
      if(projectFile == null) {
        MessageBox.Show("File not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
      }

      if(EditorManager.GetInstance(_solution).OpenProjectFile(projectFile, true) == null) {
        MessageBox.Show("File not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    #endregion

    ///<summary>
    /// Updates action visual presentation. If presentation.Enabled is set to false, Execute
    /// will not be called.
    ///</summary>
    ///<param name="context"><c>DataContext</c></param>
    ///<param name="presentation">presentation to update</param>
    ///<param name="nextUpdate">delegate to call</param>
    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate) {
      return context.CheckAllNotNull(DataConstants.SOLUTION);
    }

    ///<summary>
    /// Executes action. Called after Update, that set <see cref="ActionPresentation"/>.Enabled to true.
    ///</summary>
    ///<param name="context"><c>DataContext</c></param>
    ///<param name="nextExecute">delegate to call</param>
    public void Execute(IDataContext context, DelegateExecute nextExecute) {
      ISolution solution = context.GetData(DataConstants.SOLUTION);
      if(solution == null) {
        return;
      }

      Execute(solution, context);
    }
  }
}