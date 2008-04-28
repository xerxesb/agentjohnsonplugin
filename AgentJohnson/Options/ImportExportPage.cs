using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using AgentJohnson.Strings;
using AgentJohnson.ValueAnalysis;
using JetBrains.UI.Options;

namespace AgentJohnson.Options {
  /// <summary>
  /// 
  /// </summary>
  [OptionsPage(NAME, "Agent Johnson Settings", "AgentJohnson.Resources.Shades.gif", ParentId = "Csharp", Sequence = 1)]
  public partial class ImportExportPage : UserControl, IOptionsPage {
    #region Constants

    public const string NAME = "AgentJohnson.ImportExportPage";

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ImportExportPage"/> class.
    /// </summary>
    public ImportExportPage() {
      InitializeComponent();
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Invoked when OK button in the options dialog is pressed.
    /// If the page returns <c>false</c>, the the options dialog won't be closed, and focus will be put into this page.
    /// </summary>
    /// <returns></returns>
    public bool OnOk() {
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

    #region Private methods

    /// <summary>
    /// Handles the Export_ click event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void Export_Click(object sender, EventArgs e) {
      if(SaveFileDialog.ShowDialog() != DialogResult.OK) {
        return;
      }

      if(IntroduceStringConstantOptionsPage.Instance != null) {
        IntroduceStringConstantOptionsPage.Instance.Commit();
      }
      if(ValueAnalysisOptionsPage.Instance != null) {
        ValueAnalysisOptionsPage.Instance.Commit();
      }

      string filename = SaveFileDialog.FileName;

      StringWriter stringWriter = new StringWriter();
      XmlTextWriter writer = new XmlTextWriter(stringWriter);

      writer.WriteStartElement("settings");

      ValueAnalysisSettings.Instance.WriteSettings(writer);
      IntroduceStringConstantSettings.Instance.WriteSettings(writer);

      writer.WriteEndElement();

      File.WriteAllText(filename, stringWriter.ToString());
    }

    /// <summary>
    /// Handles the Import_ click event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void Import_Click(object sender, EventArgs e) {
      if(OpenFileDialog.ShowDialog() != DialogResult.OK) {
        return;
      }

      string filename = OpenFileDialog.FileName;

      XmlDocument doc = new XmlDocument();

      doc.Load(filename);

      ValueAnalysisSettings.Instance.ReadSettings(doc);
      IntroduceStringConstantSettings.Instance.ReadSettings(doc);

      if(IntroduceStringConstantOptionsPage.Instance != null) {
        IntroduceStringConstantOptionsPage.Instance.Display();
      }

      if(ValueAnalysisOptionsPage.Instance != null) {
        ValueAnalysisOptionsPage.Instance.Display();
      }
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
  }
}