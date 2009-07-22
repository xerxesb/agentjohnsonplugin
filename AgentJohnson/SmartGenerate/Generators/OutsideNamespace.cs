// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutsideNamespace.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The outside namespace.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.Generators
{
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// The outside namespace.
  /// </summary>
  [SmartGenerate("Generate namespace", "Generates a new namespace.", Priority = 0)]
  public class OutsideNamespace : SmartGenerateHandlerBase
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
      var namespaceDeclaration = smartGenerateParameters.Element.GetContainingElement(typeof(INamespaceDeclaration), true);
      if (namespaceDeclaration != null)
      {
        return;
      }

      this.AddAction("Namespace", "63CBED21-2B8A-4722-B585-6F90C35BC0E5");
    }

    #endregion
  }
}