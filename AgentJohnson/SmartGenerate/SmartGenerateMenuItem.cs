using System;
using JetBrains.Util;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// 
  /// </summary>
  public class SmartGenerateMenuItem : ISmartGenerateMenuItem {
    #region Public properties

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    /// <value>The tag.</value>
    public object Tag { get; set; }

    #endregion

    #region Public methods

    /// <summary>
    /// Called when the item is clicked.
    /// </summary>
    /// <returns>
    /// 	<c>true</c>, if handled, otherwise <c>false</c>.
    /// </returns>
    public bool HandleClick(object sender, EventArgs e) {
      if (Clicked != null) {
        Clicked(sender, e);
        return true;
      }

      return false;
    }

    #endregion

    #region ISmartGenerateMenuItem Members

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

    /// <summary>
    /// 
    /// </summary>
    public event EventHandler Clicked;
  }
}