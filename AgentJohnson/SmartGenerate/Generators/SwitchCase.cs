// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchCase.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The switch case.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.Generators
{
  using JetBrains.ReSharper.Psi.CSharp.Tree;

  /// <summary>
  /// The switch case.
  /// </summary>
  [SmartGenerate("'case'", "Adds a new 'case' entry in a 'switch' statement.", Priority = 0)]
  public class SwitchCase : SmartGenerateHandlerBase
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

      var switchStatement = element.GetContainingElement(typeof(ISwitchStatement), false) as ISwitchStatement;
      if (switchStatement == null)
      {
        return;
      }

      var block = switchStatement.Block;
      if (block == null)
      {
        return;
      }

      if (element.ToTreeNode().Parent != block)
      {
        return;
      }

      this.AddAction("'case'", "16E39695-5810-4C3E-A3CD-AB0CC0127C60");
    }

    #endregion
  }
}