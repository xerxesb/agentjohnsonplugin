using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// </summary>
  [SmartGenerate("Generate class members", "Generates a new property or method on a class.", Priority=0)]
  public class GenerateClassMembers : SmartGenerateBase {
    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    /// <param name="element">The element.</param>
    /// <returns>The items.</returns>
    protected override void GetItems(ISolution solution, IDataContext context, IElement element) {
      IClassDeclaration classDeclaration = element.GetContainingElement(typeof(IClassDeclaration), true) as IClassDeclaration;
      if(classDeclaration == null) {
        return;
      }

      IElement memberDeclaration = element.GetContainingElement(typeof(IClassMemberDeclaration), true);
      if(memberDeclaration != null && !(memberDeclaration is IClassDeclaration)) {
        return;
      }

      string modifier = GetModifier(element, classDeclaration);

      AddMenuItem("Auto property", "166BE49C-D068-476D-BC9C-2B5C3AF21B06", modifier);
      AddMenuItem("Property", "a684b217-f179-431b-a485-e3d76dbe57fd", modifier);
      AddMenuItem("Method", "85BBC654-4EE4-4932-BB0C-E0670FA1BB82", modifier);
    }

    #endregion
  }
}