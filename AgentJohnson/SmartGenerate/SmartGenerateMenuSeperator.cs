// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmartGenerateMenuSeperator.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The smart generate menu separator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate
{
  using System;
  using JetBrains.Util;

  /// <summary>
  /// The smart generate menu separator.
  /// </summary>
  public class SmartGenerateMenuSeparator : ISmartGenerateAction
  {
    #region Properties

    /// <summary>
    /// Gets or sets the selection range.
    /// </summary>
    /// <value>The selection range.</value>
    public TextRange SelectionRange
    {
      get
      {
        return TextRange.InvalidRange;
      }

      set
      {
      }
    }

    /// <summary>
    /// Gets or sets the template.
    /// </summary>
    /// <value>The template.</value>
    public string Template
    {
      get
      {
        return string.Empty;
      }

      set
      {
      }
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text
    {
      get
      {
        return "-";
      }

      set
      {
      }
    }

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
      return false;
    }

    #endregion

    #endregion
  }
}