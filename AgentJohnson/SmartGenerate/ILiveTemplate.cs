// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILiveTemplate.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The i live template.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate
{
  using System.Collections.Generic;

  /// <summary>
  /// The i live template.
  /// </summary>
  public interface ILiveTemplate
  {
    #region Public Methods

    /// <summary>
    /// Gets the name of the template.
    /// </summary>
    /// <param name="parameters">
    /// The parameters.
    /// </param>
    /// <returns>
    /// The items.
    /// </returns>
    IEnumerable<LiveTemplateItem> GetItems(SmartGenerateParameters parameters);

    #endregion
  }
}