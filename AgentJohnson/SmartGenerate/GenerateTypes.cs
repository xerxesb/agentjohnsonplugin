using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// </summary>
  [SmartGenerate("Generate types", "Generates a class, enum, interface or struct.", Priority=0)]
  public class GenerateTypes : SmartGenerateBase {
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

      AddMenuItem("Class", "0BC4B773-20A9-4F12-B486-5C5DB7D39C73");
      AddMenuItem("Enum", "3B6DA53E-E57F-4A22-ACF6-55F65645AF92");
      AddMenuItem("Interface", "7D2E8D45-8562-45DD-8415-3E98F0EC24BD");
      AddMenuItem("Struct", "51BFA78B-7FDA-42CC-85E4-8B29BB1103E5");
    }

    #endregion
  }
}