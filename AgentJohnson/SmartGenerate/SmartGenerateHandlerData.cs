// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmartGenerateHandlerData.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the smart generate handler data class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate
{
  /// <summary>
  /// Defines the smart generate handler data class.
  /// </summary>
  internal class SmartGenerateHandlerData
  {
    #region Properties

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the handler.
    /// </summary>
    /// <value>The handler.</value>
    public ISmartGenerateHandler Handler { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the priority.
    /// </summary>
    /// <value>The priority.</value>
    public int Priority { get; set; }

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