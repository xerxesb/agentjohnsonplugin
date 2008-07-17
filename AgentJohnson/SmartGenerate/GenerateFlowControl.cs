using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// </summary>
  [SmartGenerate(Priority=200)]
  public class GenerateFlowControl : SmartGenerateBase {
    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    /// <param name="element">The element.</param>
    /// <returns>The items.</returns>
    protected override void GetItems(ISolution solution, IDataContext context, IElement element) {
      bool hasLoop = element.GetContainingElement(typeof(IForeachStatement), false) as IForeachStatement != null;
      hasLoop |= element.GetContainingElement(typeof(IForStatement), false) as IForStatement != null;
      hasLoop |= element.GetContainingElement(typeof(IWhileStatement), false) as IWhileStatement != null;
      hasLoop |= element.GetContainingElement(typeof(IDoStatement), false) as IDoStatement != null;

      if(!hasLoop) {
        return;
      }

      if(!IsAfterLastStatement(element)) {
        return;
      }

      AddMenuItem("'continue'", "F849A86C-A93E-4805-B8E1-4B02CA8807CC");
      AddMenuItem("'break'", "42DA21AD-1F9F-4ECE-B5F6-E7AFC5EAAE14");
    }

    #endregion
  }
}