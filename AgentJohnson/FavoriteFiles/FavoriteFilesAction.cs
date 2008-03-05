using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AgentJohnson.FavoriteFiles;
using EnvDTE;
using JetBrains.ActionManagement;
using JetBrains.CommonControls;
using JetBrains.ProjectModel;
using JetBrains.ReSharper;
using JetBrains.ReSharper.Editor;
using JetBrains.ReSharper.EditorManager;
using JetBrains.Shell.VSIntegration;
using JetBrains.UI.PopupMenu;
using JetBrains.Util;
using JetBrains.UI.RichText;

namespace AgentJohnson.FavoriteFiles {
  /// <summary>
  /// Handles Find Text action, see Actions.xml
  /// </summary>
  [ActionHandler("FavoriteFiles")]
  public class FavoriteFilesAction: IActionHandler {
    #region Fields

    FavoriteFilePath _currentFile;
    ISolution _solution;

    #endregion    

    #region Protected methods

    /// <summary>
    /// Executes action. Called after Update, that set ActionPresentation.Enabled to true.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    protected void Execute(ISolution solution, IDataContext context) {
      _solution = solution;
      _currentFile = GetCurrentFile(_solution, context);

      ArrayList items = new ArrayList();

      List<FavoriteFilePath> files = FavoriteFilesSettings.Instance.FavoriteFiles;

      foreach(FavoriteFilePath favoriteFilePath in files){
        if (string.IsNullOrEmpty(favoriteFilePath.ProjectName)){
          items.Add(favoriteFilePath);
          continue;
        }

        IProject project = solution.GetProject(favoriteFilePath.ProjectName);

        if (project != null){
          items.Add(favoriteFilePath);
          continue;
        }
      }

      if (items.Count > 0){
        items.Add("seperator");
      }

      items.Add("add");
      items.Add("more");

      JetPopupMenu menu = new JetPopupMenu();

      menu.Caption = new PresentableItem("Favorite Files");
      menu.Items = items;
      menu.KeyboardAcceleration = KeyboardAccelerationFlags.Mnemonics;

      menu.DescribeItem += menu_DescribeItem;
      menu.ItemClicked += menu_ItemClicked;

      menu.Show();
    }

    #endregion

    #region Events

    /// <summary>
    /// Handles the Describe Item event of the menu control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="JetBrains.UI.PopupMenu.DescribeItemEventArgs"/> instance containing the event data.</param>
    void menu_DescribeItem(object sender, DescribeItemEventArgs e) {
      string key = e.Key as string;

      if(key != null) {
        switch(key) {
          case "seperator":
            DescribeSeperator(e);
            break;
          case "add":
            DescribeAddMenuItem(e);
            break;
          case "more":
            DescribeMoreMenuItem(e);
            break;
        }
      }
      else {
        DescribeFavoriteFile(e);
      }
    }

    /// <summary>
    /// Handles the ItemClicked event of the menu control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="JetBrains.UI.PopupMenu.ItemEventArgs"/> instance containing the event data.</param>
    void menu_ItemClicked(object sender, ItemEventArgs e) {
      string key = e.Key as string;

      if(key != null) {
        switch(key) {
          case "add":
            AddCurrentFile();
            break;
          case "more":
            Organize();
            break;
        }
      }
      else {
        OpenFavoriteFile(e);
      }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Organizes this instance.
    /// </summary>
    static void Organize() {
      Organize dialog = new Organize();

      dialog.ShowDialog();
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
    /// <param name="e">The <see cref="JetBrains.UI.PopupMenu.DescribeItemEventArgs"/> instance containing the event data.</param>
    void DescribeAddMenuItem(DescribeItemEventArgs e) {
      e.Descriptor.Text = new RichText("Add Current File");

      if(_currentFile != null) {
        e.Descriptor.Style = MenuItemStyle.Enabled;
      }
    }

    /// <summary>
    /// Describes the favorite file.
    /// </summary>
    /// <param name="e">The <see cref="JetBrains.UI.PopupMenu.DescribeItemEventArgs"/> instance containing the event data.</param>
    static void DescribeFavoriteFile(DescribeItemEventArgs e) {
      FavoriteFilePath favoriteFilePath = e.Key as FavoriteFilePath;

      if(favoriteFilePath == null) {
        e.Descriptor.Text = "<Error>";
        return;
      }

      try{
        e.Descriptor.Text = Path.GetFileName(favoriteFilePath.Path);
      }
      catch{
        e.Descriptor.Text = favoriteFilePath.Path;
      }
      e.Descriptor.Style = MenuItemStyle.Enabled;
      e.Descriptor.ShortcutText = new RichText("(" + favoriteFilePath + ")", TextStyle.FromForeColor(Color.LightGray));


      ArrayList list = e.Menu.Items as ArrayList;
      if(list == null){
        return;
      }

      int index = list.IndexOf(e.Key);

      if (index < 0 || index > 8){
        return;
      }

      index++;

      e.Descriptor.Mnemonic = index.ToString();
    }

    /// <summary>
    /// Describes the more menu item.
    /// </summary>
    /// <param name="e">The <see cref="JetBrains.UI.PopupMenu.DescribeItemEventArgs"/> instance containing the event data.</param>
    static void DescribeMoreMenuItem(DescribeItemEventArgs e) {
      e.Descriptor.Text = "Organize...";
      e.Descriptor.Style = MenuItemStyle.Enabled;
    }

    /// <summary>
    /// Describes the seperator.
    /// </summary>
    /// <param name="e">The <see cref="JetBrains.UI.PopupMenu.DescribeItemEventArgs"/> instance containing the event data.</param>
    static void DescribeSeperator(DescribeItemEventArgs e) {
      e.Descriptor.Style = MenuItemStyle.Separator;
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
    /// <param name="e">The <see cref="JetBrains.UI.PopupMenu.ItemEventArgs"/> instance containing the event data.</param>
    void OpenFavoriteFile(ItemEventArgs e) {
      FavoriteFilePath favoriteFilePath = e.Key as FavoriteFilePath;
      if(favoriteFilePath == null) {
        return;
      }

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
    ///<param name="context">DataContext</param>
    ///<param name="presentation">presentation to update</param>
    ///<param name="nextUpdate">delegate to call</param>
    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate) {
      return context.CheckAllNotNull(DataConstants.SOLUTION);
    }

    ///<summary>
    ///
    ///            Executes action. Called after Update, that set ActionPresentation.Enabled to true.
    ///            
    ///</summary>
    ///
    ///<param name="context">DataContext</param>
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