// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportExportPage.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The import export page.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Options
{
  using System;
  using System.IO;
  using System.Windows.Forms;
  using System.Xml;
  using Strings;
  using ValueAnalysis;
  using JetBrains.UI.Options;

  /// <summary>
  /// The import export page.
  /// </summary>
  [OptionsPage(NAME, "Agent Johnson Settings", "AgentJohnson.Resources.Shades.gif", ParentId = "Csharp", Sequence = 1)]
  public partial class ImportExportPage : UserControl, IOptionsPage
  {
    #region Constants and Fields

    /// <summary>
    /// The name.
    /// </summary>
    public const string NAME = "AgentJohnson.ImportExportPage";

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportExportPage"/> class.
    /// </summary>
    public ImportExportPage()
    {
      this.InitializeComponent();
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
    /// Handles the Export_ click event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data.
    /// </param>
    private void Export_Click(object sender, EventArgs e)
    {
      if (this.SaveFileDialog.ShowDialog() != DialogResult.OK)
      {
        return;
      }

      if (IntroduceStringConstantOptionsPage.Instance != null)
      {
        IntroduceStringConstantOptionsPage.Instance.Commit();
      }

      if (ValueAnalysisOptionsPage.Instance != null)
      {
        ValueAnalysisOptionsPage.Instance.Commit();
      }

      var filename = this.SaveFileDialog.FileName;

      var stringWriter = new StringWriter();
      var writer = new XmlTextWriter(stringWriter);

      writer.WriteStartElement("settings");

      ValueAnalysisSettings.Instance.WriteSettings(writer);
      IntroduceStringConstantSettings.Instance.WriteSettings(writer);

      writer.WriteEndElement();

      File.WriteAllText(filename, stringWriter.ToString());
    }

    /// <summary>
    /// Handles the Import_ click event.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data.
    /// </param>
    private void Import_Click(object sender, EventArgs e)
    {
      if (this.OpenFileDialog.ShowDialog() != DialogResult.OK)
      {
        return;
      }

      var filename = this.OpenFileDialog.FileName;

      var doc = new XmlDocument();

      doc.Load(filename);

      ValueAnalysisSettings.Instance.ReadSettings(doc);
      IntroduceStringConstantSettings.Instance.ReadSettings(doc);

      if (IntroduceStringConstantOptionsPage.Instance != null)
      {
        IntroduceStringConstantOptionsPage.Instance.Display();
      }

      if (ValueAnalysisOptionsPage.Instance != null)
      {
        ValueAnalysisOptionsPage.Instance.Display();
      }
    }

    #endregion
  }
}