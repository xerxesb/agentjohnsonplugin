// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Types.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.Generators
{
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// The types.
  /// </summary>
  [SmartGenerate("Generate types", "Generates a class, enum, interface or struct.", Priority = 0)]
  public class Types : SmartGenerateHandlerBase
  {
    #region Methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">
    /// The get menu items parameters.
    /// </param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters)
    {
      var element = smartGenerateParameters.Element;

      var classLikeDeclaration = element.GetContainingElement(typeof(IClassLikeDeclaration), true);
      if (classLikeDeclaration != null)
      {
        return;
      }

      var enumDecl = element.GetContainingElement(typeof(IEnumDeclaration), true) as IEnumDeclaration;
      if (enumDecl != null)
      {
        return;
      }

      var namespaceDeclaration = element.GetContainingElement(typeof(INamespaceDeclaration), true);
      if (namespaceDeclaration == null)
      {
        return;
      }

      this.AddAction("Class", "0BC4B773-20A9-4F12-B486-5C5DB7D39C73");
      this.AddAction("Enum", "3B6DA53E-E57F-4A22-ACF6-55F65645AF92");
      this.AddAction("Interface", "7D2E8D45-8562-45DD-8415-3E98F0EC24BD");
      this.AddAction("Struct", "51BFA78B-7FDA-42CC-85E4-8B29BB1103E5");
    }

    #endregion
  }
}