// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExpression.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The string expression.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.Generators
{
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// The string expression.
  /// </summary>
  [SmartGenerate("Surround with 'string.IsNullOrEmpty'", "Surrounds the string expression with 'string.IsNullOrEmpty'.", Priority = -20)]
  public class StringExpression : SmartGenerateHandlerBase
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

      var expression = element.GetContainingElement(typeof(IExpression), false) as IExpression;
      while (expression != null)
      {
        var type = expression.Type();

        var typeName = type.GetPresentableName(element.Language);

        if (typeName == "string")
        {
          this.AddAction("Surround with 'string.IsNullOrEmpty'", "3D13FE3E-7004-42B0-B205-2881C5ADBAD2", expression.GetTreeTextRange());
          return;
        }

        expression = expression.GetContainingElement(typeof(IExpression), false) as IExpression;
      }
    }

    #endregion
  }
}