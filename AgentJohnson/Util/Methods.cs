using JetBrains.ReSharper.Psi;

namespace AgentJohnson.Util {
  /// <summary>
  /// 
  /// </summary>
  public static class Methods {
    #region Public methods

    /// <summary>
    /// Gets the full name of the method.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <returns></returns>
    public static string GetFullMethodName(IDeclaredElement method) {
      string result = method.ShortName;

      ITypeElement typeElement = method.GetContainingType();
      if(typeElement != null) {
        result = typeElement.ShortName + "." + result;

        INamespace ns = typeElement.GetContainingNamespace();

        result = ns.QualifiedName + "." + result;
      }

      return result;
    }

    #endregion
  }
}