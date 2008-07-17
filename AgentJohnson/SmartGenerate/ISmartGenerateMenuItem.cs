using JetBrains.Util;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// 
  /// </summary>
  public interface ISmartGenerateMenuItem {
    #region Public properties

    /// <summary>
    /// Gets or sets the selection range.
    /// </summary>
    /// <value>The selection range.</value>
    TextRange SelectionRange { get; set; }
    /// <summary>
    /// Gets or sets the template.
    /// </summary>
    /// <value>The template.</value>
    string Template { get; set; }
    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    string Text { get; set; }

    #endregion
  }
}