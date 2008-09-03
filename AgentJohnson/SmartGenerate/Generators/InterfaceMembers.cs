using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.Generators {
  /// <summary>
  /// </summary>
  [SmartGenerate("Generate interface members", "Generates a new property or method on an interface", Priority=0)]
  public class InterfaceMembers : SmartGenerateBase {
    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">The get menu items parameters.</param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters) {
      IElement element = smartGenerateParameters.Element;

      IElement interfaceDeclaration = element.GetContainingElement(typeof(IInterfaceDeclaration), true);
      if(interfaceDeclaration == null) {
        return;
      }

      IElement memberDeclaration = element.GetContainingElement(typeof(ITypeMemberDeclaration), true);
      if(memberDeclaration != null && !(memberDeclaration is IInterfaceDeclaration)) {
        return;
      }

      AddMenuItem("Property", "D6EB42DA-2858-46B3-8CB3-E3DEFB245D11");
      AddMenuItem("Method", "B3DB6158-D43E-42EE-8E67-F10CF7344106");
    }

    #endregion
  }
}