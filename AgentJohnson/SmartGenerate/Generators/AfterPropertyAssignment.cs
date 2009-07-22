// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AfterPropertyAssignment.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The after property assignment.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.Generators
{
  using JetBrains.ReSharper.Psi.CSharp.Tree;

  /// <summary>
  /// The after property assignment.
  /// </summary>
  [SmartGenerate("Assign property", "Assigns another property on the object.", Priority = 0)]
  public class AfterPropertyAssignment : SmartGenerateHandlerBase
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

      var expressionStatement = StatementUtil.GetPreviousStatement(block, element) as IExpressionStatement;
      if (expressionStatement == null)
      {
        return;
      }

      var assignmentExpression = expressionStatement.Expression as IAssignmentExpression;
      if (assignmentExpression == null)
      {
        return;
      }

      var referenceExpression = assignmentExpression.Dest as IReferenceExpression;
      if (referenceExpression == null)
      {
        return;
      }

      var reference = referenceExpression.Reference;
      if (reference == null)
      {
        return;
      }

      var resolve = reference.Resolve();

      var propertyDeclaration = resolve.DeclaredElement as IPropertyDeclaration;
      if (propertyDeclaration == null)
      {
        return;
      }

      var qualifierExpression = referenceExpression.QualifierExpression;
      if (qualifierExpression == null)
      {
        return;
      }

      var qualifier = qualifierExpression.GetText();

      this.AddAction("Assign another property on '{0}'", "AA6EFC53-174B-4EFD-A137-8115C22666C7", qualifier);
    }

    #endregion
  }
}