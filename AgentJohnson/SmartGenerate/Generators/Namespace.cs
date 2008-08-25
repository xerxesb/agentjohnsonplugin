using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.Generators {
  /// <summary>
  /// </summary>
  [SmartGenerate("Generate namespace", "Generates a new namespace.", Priority=0)]
  public class Namespace : SmartGenerateBase {
    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    /// <param name="element">The element.</param>
    /// <returns>The items.</returns>
    protected override void GetItems(ISolution solution, IDataContext context, IElement element) {
      IElement namespaceDeclaration = element.GetContainingElement(typeof(INamespaceDeclaration), true);
      if(namespaceDeclaration != null) {
        return;
      }

      AddMenuItem("Namespace", "63CBED21-2B8A-4722-B585-6F90C35BC0E5");
    }

    #endregion
  }
}