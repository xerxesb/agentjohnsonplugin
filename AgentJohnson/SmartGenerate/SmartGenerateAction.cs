// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmartGenerateAction.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The smart generate action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate
{
  using System;
  using JetBrains.Util;

  /// <summary>
  /// The smart generate action.
  /// </summary>
  public class SmartGenerateAction : ISmartGenerateAction
  {
    #region Events

    /// <summary>
    /// The clicked.
    /// </summary>
    public event EventHandler Clicked;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the selection range.
    /// </summary>
    /// <value>The selection range.</value>
    public TextRange SelectionRange { get; set; }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    /// <value>The tag.</value>
    public object Tag { get; set; }

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

    #region Implemented Interfaces

    #region ISmartGenerateAction

    /// <summary>
    /// Called when the item is clicked.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    /// <returns>
    /// <c>true</c>, if handled, otherwise <c>false</c>.
    /// </returns>
    public bool HandleClick(object sender, EventArgs e)
    {
      if (this.Clicked != null)
      {
        this.Clicked(sender, e);
        return true;
      }

      return false;
    }

    #endregion

    #endregion
  }
}