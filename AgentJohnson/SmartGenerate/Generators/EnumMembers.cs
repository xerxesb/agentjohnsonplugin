using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.Generators {
  /// <summary>
  /// </summary>
  [SmartGenerate("Add enum member", "Adds a new constant to the enumeration.", Priority=0)]
  public class EnumMembers : SmartGenerateBase {
    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">The get menu items parameters.</param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters) {
      IElement element = smartGenerateParameters.Element;

      IEnumDeclaration enumDeclaration = element.GetContainingElement(typeof(IEnumDeclaration), false) as IEnumDeclaration;
      if(enumDeclaration == null) {
        return;
      }

      AddMenuItem("Add enum member", "587F88E2-6876-41F2-885C-58AD93BBC8B4");
    }

    #endregion
  }
}