using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// </summary>
  [SmartGenerate(Priority=0)]
  public class GenerateClasses : SmartGenerateBase {
    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    /// <param name="element">The element.</param>
    /// <returns>The items.</returns>
    protected override void GetItems(ISolution solution, IDataContext context, IElement element) {
      IElement classLikeDeclaration = element.GetContainingElement(typeof(IClassLikeDeclaration), true);
      if(classLikeDeclaration != null) {
        return;
      }

      IEnumDeclaration enumDecl = element.GetContainingElement(typeof(IEnumDeclaration), true) as IEnumDeclaration;
      if(enumDecl != null) {
        return;
      }

      IElement namespaceDeclaration = element.GetContainingElement(typeof(INamespaceDeclaration), true);
      if(namespaceDeclaration == null) {
        return;
      }

      AddMenuItem("Generate class", "0BC4B773-20A9-4F12-B486-5C5DB7D39C73");
      AddMenuItem("Generate enum", "3B6DA53E-E57F-4A22-ACF6-55F65645AF92");
      AddMenuItem("Generate interface", "7D2E8D45-8562-45DD-8415-3E98F0EC24BD");
      AddMenuItem("Generate struct", "51BFA78B-7FDA-42CC-85E4-8B29BB1103E5");
    }

    #endregion
  }
}