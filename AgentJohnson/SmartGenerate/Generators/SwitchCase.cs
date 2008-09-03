using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.Generators {
  /// <summary>
  /// </summary>
  [SmartGenerate("'case'", "Adds a new 'case' entry in a 'switch' statement.", Priority = 0)]
  public class SwitchCase : SmartGenerateBase {
    #region Protected methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">The get menu items parameters.</param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters) {
      IElement element = smartGenerateParameters.Element;

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