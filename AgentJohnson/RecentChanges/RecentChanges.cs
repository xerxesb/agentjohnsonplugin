// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecentChanges.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the recent changes class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.RecentChanges
{
  using System;
  using System.IO;
  using System.Windows.Forms;
  using JetBrains.IDE;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Features.Environment.RecentFiles;

  /// <summary>
  /// Defines the recent changes class.
  /// </summary>
  public partial class RecentChanges : Form
  {
    #region Constants and Fields

    /// <summary>
    /// The solution.
    /// </summary>
    private ISolution solution;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="RecentChanges"/> class. 
    /// </summary>
    public RecentChanges()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the selected text.
    /// </summary>
    /// <value>The selected text.</value>
    public string SelectedText { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Loads the specified solution.
    /// </summary>
    /// <param name="solution">
    /// The solution.
    /// </param>
    public void LoadChanges(ISolution solution)
    {
      this.solution = solution;

      var locations = RecentFilesManager.GetInstance(solution).EditLocations;

      var count = 0;
      foreach (var locationInfo in locations)
      {
        var projectFile = locationInfo.ProjectFile;
        var project = projectFile.GetProject();

        var solutionName = project.GetSolution().Name;
        var fileName = projectFile.Location.ConvertToRelativePath(project.Location).FullPath;

        int line;
        int position;
        int selectionStart;
        int selectionLength;

        if (EditorManager.GetInstance(solution).IsOpenedInTextControl(projectFile))
        {
          var textControl = EditorManager.GetInstance(solution).GetTextControl(projectFile);

          var documentText = textControl.Document.GetText();

          ReadText(new StringReader(documentText), locationInfo.CaretOffset, out line, out position, out selectionStart, out selectionLength);
        }
        else
        {
          ReadFile(projectFile, locationInfo.CaretOffset, out line, out position, out selectionStart, out selectionLength);
        }

        var labelText = "<" + solutionName + ">\\" + fileName + " (Ln " + line + ", Col " + position + ")";

        var l = new Selection
        {
          Text = labelText,
          SelectionStart = selectionStart,
          SelectionLength = selectionLength,
        };

        this.Listbox.Items.Add(l);

        count++;
        if (count >= 25)
        {
          break;
        }
      }

      if (this.Listbox.Items.Count > 0)
      {
        this.Listbox.SelectedIndex = 0;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Reads the file.
    /// </summary>
    /// <param name="projectFile">
    /// The project file.
    /// </param>
    /// <param name="caretOffset">
    /// The caret offset.
    /// </param>
    /// <param name="line">
    /// The current line.
    /// </param>
    /// <param name="position">
    /// The position.
    /// </param>
    /// <param name="selectionStart">
    /// The selection start.
    /// </param>
    /// <param name="selectionLength">
    /// Length of the selection.
    /// </param>
    private static void ReadFile(IProjectFile projectFile, int caretOffset, out int line, out int position, out int selectionStart, out int selectionLength)
    {
      using (var stream = projectFile.CreateReadStream())
      {
        var reader = new StreamReader(stream);

        ReadText(reader, caretOffset, out line, out position, out selectionStart, out selectionLength);
      }
    }

    /// <summary>
    /// Reads the text.
    /// </summary>
    /// <param name="reader">
    /// The reader.
    /// </param>
    /// <param name="caretOffset">
    /// The caret offset.
    /// </param>
    /// <param name="line">
    /// The current line.
    /// </param>
    /// <param name="position">
    /// The position.
    /// </param>
    /// <param name="selectionStart">
    /// The selection start.
    /// </param>
    /// <param name="selectionLength">
    /// Length of the selection.
    /// </param>
    private static void ReadText(TextReader reader, int caretOffset, out int line, out int position, out int selectionStart, out int selectionLength)
    {
      line = 1;
      position = 0;
      selectionStart = 0;
      selectionLength = 0;

      var offset = 0;

      var s = reader.ReadLine();
      while (s != null)
      {
        offset += s.Length + 2;

        if (offset >= caretOffset)
        {
          break;
        }

        line++;
        s = reader.ReadLine();
      }

      if (s == null)
      {
        return;
      }

      selectionStart = offset - s.Length;
      selectionLength = s.Length;
      position = caretOffset - (offset - s.Length) + 1;
    }

    /// <summary>
    /// Accepts this instance.
    /// </summary>
    private void Accept()
    {
      var text = this.Preview.SelectedText;

      Clipboard.SetText(text);

      this.SelectedText = text;

      this.DialogResult = DialogResult.OK;

      this.Close();
    }

    /// <summary>
    /// Codes the on key down.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="args">
    /// The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.
    /// </param>
    private void CodeOnKeyDown(object sender, KeyEventArgs args)
    {
      if (args.KeyCode == Keys.Escape)
      {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
      }

      if (args.KeyCode == Keys.Enter)
      {
        this.Accept();
      }
    }

    /// <summary>
    /// Handles the SelectedIndexChanged event of the <c>Listbox</c> control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data.
    /// </param>
    private void Listbox_SelectedIndexChanged(object sender, EventArgs e)
    {
      var selectedIndex = this.Listbox.SelectedIndex;
      if (selectedIndex < 0)
      {
        this.Preview.Text = string.Empty;
        return;
      }

      var l = this.Listbox.SelectedItem as Selection;
      if (l == null)
      {
        return;
      }

      var locations = RecentFilesManager.GetInstance(this.solution).EditLocations;

      var locationInfo = locations[selectedIndex];
      if (locationInfo == null)
      {
        return;
      }

      var projectFile = locationInfo.ProjectFile;

      if (EditorManager.GetInstance(this.solution).IsOpenedInTextControl(projectFile))
      {
        var textControl = EditorManager.GetInstance(this.solution).GetTextControl(projectFile);

        this.Preview.Text = textControl.Document.GetText();
      }
      else
      {
        using (var stream = projectFile.CreateReadStream())
        {
          var reader = new StreamReader(stream);
          this.Preview.Text = reader.ReadToEnd();
        }
      }

      this.Preview.SelectionStart = l.SelectionStart;
      this.Preview.SelectionLength = l.SelectionLength;
      this.Preview.ScrollToCaret();
    }

    #endregion

    /// <summary>
    /// Defines the location class.
    /// </summary>
    private class Selection
    {
      #region Properties

      /// <summary>
      /// Gets or sets the length of the selection.
      /// </summary>
      /// <value>The length of the selection.</value>
      public int SelectionLength { get; set; }

      /// <summary>
      /// Gets or sets the selection start.
      /// </summary>
      /// <value>The selection start.</value>
      public int SelectionStart { get; set; }

      /// <summary>
      /// Gets or sets the text.
      /// </summary>
      /// <value>The text.</value>
      public string Text { get; set; }

      #endregion

      #region Public Methods

      /// <summary>
      /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </summary>
      /// <returns>
      /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
      /// </returns>
      public override string ToString()
      {
        return this.Text;
      }

      #endregion
    }
  }
}