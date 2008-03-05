using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AgentJohnson.FavoriteFiles;

namespace AgentJohnson.FavoriteFiles {
  /// <summary>
  /// Represents the Organize dialog
  /// </summary>
  public partial class Organize : Form {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="Organize"/> class.
    /// </summary>
    public Organize() {
      InitializeComponent();

      List<FavoriteFilePath> files = new List<FavoriteFilePath>(FavoriteFilesSettings.Instance.FavoriteFiles);

      Listbox.BeginUpdate();

      foreach(FavoriteFilePath file in files){
        Listbox.Items.Add(file);
      }

      Listbox.EndUpdate();

      EnableButtons();
    }

    #endregion

    #region Events

    /// <summary>
    /// Handles the Up button_ click event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void UpButton_Click(object sender, EventArgs e) {
      int n = Listbox.SelectedIndex;

      if (n < 1){
        return;
      }

      object selected = Listbox.Items[n];

      Listbox.BeginUpdate();
      
      Listbox.Items.RemoveAt(n);
      Listbox.Items.Insert(n - 1, selected);

      Listbox.EndUpdate();

      Listbox.SelectedIndex = n - 1;
    }

    /// <summary>
    /// Handles the Down button_ click event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void DownButton_Click(object sender, EventArgs e) {
      int n = Listbox.SelectedIndex;

      if(n < 0 || n >= Listbox.Items.Count - 1) {
        return;
      }

      object selected = Listbox.Items[n];

      Listbox.BeginUpdate();

      Listbox.Items.RemoveAt(n);
      Listbox.Items.Insert(n + 1, selected);

      Listbox.EndUpdate();

      Listbox.SelectedIndex = n + 1;
    }

    /// <summary>
    /// Handles the Delete button_ click event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void DeleteButton_Click(object sender, EventArgs e) {
      int n = Listbox.SelectedIndex;

      if(n < 0) {
        return;
      }

      Listbox.BeginUpdate();

      Listbox.Items.RemoveAt(n);

      Listbox.EndUpdate();

      if (n >= Listbox.Items.Count){
        n--;
      }

      Listbox.SelectedIndex = n;
    }

    /// <summary>
    /// Handles the OK button_ click event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void OKButton_Click(object sender, EventArgs e) {
      List<FavoriteFilePath> files = new List<FavoriteFilePath>();

      foreach(FavoriteFilePath file in Listbox.Items) {
        files.Add(file);
      }

      FavoriteFilesSettings.Instance.FavoriteFiles = files;
    }

    /// <summary>
    /// Handles the SelectedIndexChanged event of the Listbox control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void Listbox_SelectedIndexChanged(object sender, EventArgs e) {
      EnableButtons();
    }

    /// <summary>
    /// Enables the buttons.
    /// </summary>
    void EnableButtons() {
      int n = Listbox.SelectedIndex;

      DeleteButton.Enabled = n >= 0;
      UpButton.Enabled = n > 0;
      DownButton.Enabled = n >= 0 && n < Listbox.Items.Count - 1;
    }

    #endregion  

  }
}