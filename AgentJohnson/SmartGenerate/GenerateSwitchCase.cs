using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// </summary>
  [SmartGenerate(Priority=0)]
  public class GenerateSwitchCase : SmartGenerateBase {
    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    /// <param name="element">The element.</param>
    /// <returns>The items.</returns>
    protected override void GetItems(ISolution solution, IDataContext context, IElement element) {
      ISwitchStatement switchStatement = element.GetContainingElement(typeof(ISwitchStatement), false) as ISwitchStatement;
      if(switchStatement == null) {
        return;
      }

      IBlock block = switchStatement.Block;
      if(block == null) {
        return;
      }

      if(element.ToTreeNode().Parent != block) {
        return;
      }

      AddMenuItem("'case'", "16E39695-5810-4C3E-A3CD-AB0CC0127C60");
    }

    #endregion
  }
}