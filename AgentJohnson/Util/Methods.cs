// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Methods.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Util
{
  using JetBrains.ReSharper.Psi;

  /// <summary>
  /// The methods.
  /// </summary>
  public static class Methods
  {
    #region Public Methods

    /// <summary>
    /// Gets the full name of the method.
    /// </summary>
    /// <param name="method">
    /// The method.
    /// </param>
    /// <returns>
    /// The get full method name.
    /// </returns>
    public static string GetFullMethodName(IDeclaredElement method)
    {
      var result = method.ShortName;

      var typeElement = method.GetContainingType();
      if (typeElement != null)
      {
        result = typeElement.ShortName + "." + result;

        var ns = typeElement.GetContainingNamespace();

        result = ns.QualifiedName + "." + result;
      }

      return result;
    }

    #endregion
  }
}