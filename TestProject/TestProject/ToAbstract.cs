// <copyright file="ToAbstract.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace TestProject
{
  using System;

  /// <summary>
  /// Defines to override class.
  /// </summary>
  public class ToAbstract
  {
    #region Public properties

    /// <summary>
    /// Gets or sets my string.
    /// </summary>
    /// <value>My string.</value>
    public virtual string MyString
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Gets the string.
    /// </summary>
    /// <returns>Returns the boolean.</returns>
    public virtual bool GetString()
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}