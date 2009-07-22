// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISmartGenerateHandler.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The i smart generate handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate
{
  using System.Collections.Generic;

  /// <summary>
  /// The i smart generate handler.
  /// </summary>
  public interface ISmartGenerateHandler
  {
    #region Public Methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="parameters">
    /// The parameters.
    /// </param>
    /// <returns>
    /// The menu items.
    /// </returns>
    /// <value>
    /// The items.
    /// </value>
    IEnumerable<ISmartGenerateAction> GetMenuItems(SmartGenerateParameters parameters);

    #endregion
  }
}