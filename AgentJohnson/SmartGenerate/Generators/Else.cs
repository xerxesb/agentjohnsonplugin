namespace AgentJohnson.SmartGenerate.Generators
{
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// </summary>
  [SmartGenerate("Generate else", "Generates an 'else' or 'else if' statement.", Priority = 100)]
  public class Else : SmartGenerateHandlerBase
  {
    #region Protected methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">The get menu items parameters.</param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters)
    {
      IElement element = smartGenerateParameters.Element;

      IBlock block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if (block == null)
      {
        return;
      }
      IElement statement = element.GetContainingElement(typeof(IStatement), true);
      if (statement != null && !block.Contains(statement))
      {
        return;
      }

      IIfStatement ifStatement = StatementUtil.GetPreviousStatement(block, element) as IIfStatement;
      if (ifStatement == null)
      {
        return;
      }

      IStatement elseStatement = ifStatement.Else;
      while (elseStatement != null && elseStatement is IIfStatement)
      {
        elseStatement = (elseStatement as IIfStatement).Else;
      }
      if (elseStatement != null)
      {
        return;
      }

      this.AddAction("'else'", "9F134F1B-3F0D-4C9E-B549-A469828D1A7F");
      this.AddAction("'else if'", "94F834F9-110D-4608-A780-9BD05FE826A1");
    }

    #endregion
  }
}