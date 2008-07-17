using JetBrains.Util;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// 
  /// </summary>
  public class SmartGenerateMenuItem : ISmartGenerateMenuItem {
    #region Public properties

    /// <summary>
    /// Gets or sets the selection range.
    /// </summary>
    /// <value>The selection range.</value>
    public TextRange SelectionRange { get; set; }

    /// <summary>
    /// Gets or sets the template.
    /// </summary>
    /// <value>The template.</value>
    public string Template { get; set; }
    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text { get; set; }

    #endregion
  }
}