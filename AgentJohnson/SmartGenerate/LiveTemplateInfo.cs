// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiveTemplateInfo.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The live template info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate
{
  using System;

  /// <summary>
  /// The live template info.
  /// </summary>
  internal class LiveTemplateInfo
  {
    #region Properties

    /// <summary>
    /// Gets or sets Description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets Name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets Priority.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Gets or sets Type.
    /// </summary>
    public Type Type { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Returns the fully qualified type name of this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> containing a fully qualified type name.
    /// </returns>
    public override string ToString()
    {
      return this.Name;
    }

    #endregion
  }
}