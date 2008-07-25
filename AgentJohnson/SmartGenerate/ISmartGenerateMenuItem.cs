using System;
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

    #region Public methods

    /// <summary>
    /// Called when the item is clicked.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    /// <returns>
    /// 	<c>true</c>, if handled, otherwise <c>false</c>.
    /// </returns>
    bool HandleClick(object sender, EventArgs e);

    #endregion
  }
}