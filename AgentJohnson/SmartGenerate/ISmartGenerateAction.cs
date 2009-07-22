// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISmartGenerateAction.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The i smart generate action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate
{
  using System;
  using JetBrains.Util;

  /// <summary>
  /// The i smart generate action.
  /// </summary>
  public interface ISmartGenerateAction
  {
    #region Properties

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

    #region Public Methods

    /// <summary>
    /// Called when the item is clicked.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data.
    /// </param>
    /// <returns>
    /// <c>true</c>, if handled, otherwise <c>false</c>.
    /// </returns>
    bool HandleClick(object sender, EventArgs e);

    #endregion
  }
}