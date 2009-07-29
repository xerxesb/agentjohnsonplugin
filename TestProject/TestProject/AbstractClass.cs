// <copyright file="AbstractClass.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace TestProject
{
  /// <summary>
  /// Defines the abstract class class.
  /// </summary>
  public abstract class AbstractClass
  {
    #region Public properties

    /// <summary>
    /// Gets or sets the abstract property.
    /// </summary>
    /// <value>The abstract property.</value>
    public abstract string AbstractProperty { get; set; }

    #endregion

    #region Public methods

    /// <summary>
    /// Abstracts the method.
    /// </summary>
    public abstract void AbstractMethod();

    #endregion
  }
}