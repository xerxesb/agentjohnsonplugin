// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Else.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The else.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.Generators
{
  using JetBrains.ReSharper.Psi.CSharp.Tree;

  /// <summary>
  /// The else.
  /// </summary>
  [SmartGenerate("Generate else", "Generates an 'else' or 'else if' statement.", Priority = 100)]
  public class Else : SmartGenerateHandlerBase
  {
    #region Methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">
    /// The get menu items parameters.
    /// </param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters)
    {
      var element = smartGenerateParameters.Element;

      var block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if (block == null)
      {
        return;
      }

      var statement = element.GetContainingElement(typeof(IStatement), true);
      if (statement != null && !block.Contains(statement))
      {
        return;
      }

      var ifStatement = StatementUtil.GetPreviousStatement(block, element) as IIfStatement;
      if (ifStatement == null)
      {
        return;
      }

      var elseStatement = ifStatement.Else;
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