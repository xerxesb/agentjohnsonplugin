using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// </summary>
  [SmartGenerate("Add enum member", "Adds a new constant to the enumeration.", Priority=0)]
  public class GenerateEnumMembers : SmartGenerateBase {
    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    /// <param name="element">The element.</param>
    /// <returns>The items.</returns>
    protected override void GetItems(ISolution solution, IDataContext context, IElement element) {
      IEnumDeclaration enumDeclaration = element.GetContainingElement(typeof(IEnumDeclaration), false) as IEnumDeclaration;
      if(enumDeclaration == null) {
        return;
      }

      AddMenuItem("Add enum member", "587F88E2-6876-41F2-885C-58AD93BBC8B4");
    }

    #endregion
  }
}