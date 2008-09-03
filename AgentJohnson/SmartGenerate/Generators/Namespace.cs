using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.Generators {
  /// <summary>
  /// </summary>
  [SmartGenerate("Generate namespace", "Generates a new namespace.", Priority = 0)]
  public class Namespace : SmartGenerateBase {
    #region Protected methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">The get menu items parameters.</param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters) {
      IElement namespaceDeclaration = smartGenerateParameters.Element.GetContainingElement(typeof(INamespaceDeclaration), true);
      if(namespaceDeclaration != null) {
        return;
      }

      AddMenuItem("Namespace", "63CBED21-2B8A-4722-B585-6F90C35BC0E5");
    }

    #endregion
  }
}