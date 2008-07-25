using System;
using JetBrains.Util;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// 
  /// </summary>
  public class SmartGenerateMenuSeparator : ISmartGenerateMenuItem {
    /// <summary>
    /// 
    /// </summary>
    public event EventHandler Clicked;

    #region Public properties

    /// <summary>
    /// Gets or sets the selection range.
    /// </summary>
    /// <value>The selection range.</value>
    public TextRange SelectionRange {
      get {
        return TextRange.InvalidRange;
      }
      set {
      }
    }

    /// <summary>
    /// Gets or sets the template.
    /// </summary>
    /// <value>The template.</value>
    public string Template {
      get {
        return string.Empty;
      }
      set {
      }
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text {
      get {
        return "-";
      }
      set {
      }
    }

    /// <summary>
    /// Called when the item is clicked.
    /// </summary>
    /// <returns>
    /// 	<c>true</c>, if handled, otherwise <c>false</c>.
    /// </returns>
    public bool HandleClick(object sender, EventArgs e) {
      return false;
    }

    #endregion
  }
}