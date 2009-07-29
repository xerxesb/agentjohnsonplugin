// <copyright file="ProxyClass.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace TestProject
{
  /// <summary>
  /// Defines the proxy class class.
  /// </summary>
  public class ProxyClass
  {
    #region Fields

    /// <summary>
    /// The string property.
    /// </summary>
    private string stringProperty;

    #endregion

    #region Public properties

    /// <summary>
    /// Gets or sets the string property.
    /// </summary>
    /// <value>The string property.</value>
    public string StringProperty
    {
      get
      {
        return this.stringProperty;
      }

      set
      {
        this.stringProperty = value;
      }
    }

    #endregion
  }

  /// <summary>
  /// Defines the static proxy class class.
  /// </summary>
  public static class StaticProxyClass
  {
    #region Public properties

    /// <summary>
    /// Gets or sets the static string property.
    /// </summary>
    /// <value>The static string property.</value>
    public static string StaticStringProperty { get; set; }

    /// <summary>
    /// Refs the method.
    /// </summary>
    /// <param name="p">The parameter.</param>
    public static void RefMethod(ref string p)
    {
    }

    #endregion
  }
}