using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.Generators {
  /// <summary>
  /// </summary>
  [SmartGenerate("Generate else", "Generates an 'else' or 'else if' statement.", Priority=100)]
  public class Else : SmartGenerateBase {
    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    /// <param name="element">The element.</param>
    /// <returns>The items.</returns>
    protected override void GetItems(ISolution solution, IDataContext context, IElement element) {
      IBlock block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if(block == null) {
        return;
      }
      IElement statement = element.GetContainingElement(typeof(IStatement), true);
      if(statement != null && !block.Contains(statement)) {
        return;
      }

      IIfStatement ifStatement = StatementUtil.GetPreviousStatement(block, element) as IIfStatement;
      if(ifStatement == null) {
        return;
      }

      IStatement elseStatement = ifStatement.Else;
      while(elseStatement != null && elseStatement is IIfStatement) {
        elseStatement = (elseStatement as IIfStatement).Else;
      }
      if(elseStatement != null) {
        return;
      }

      AddMenuItem("'else'", "9F134F1B-3F0D-4C9E-B549-A469828D1A7F");
      AddMenuItem("'else if'", "94F834F9-110D-4608-A780-9BD05FE826A1");
    }

    #endregion
  }
}