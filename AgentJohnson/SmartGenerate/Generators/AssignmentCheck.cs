// <copyright file="AssignmentCheck.cs" company="Jakob Christensen">
//   Copyright (c) Jakob Christensen. All rights reserved.
// </copyright>

namespace AgentJohnson.SmartGenerate.Generators
{
  using System.Collections.Generic;
  using JetBrains.Application;
  using JetBrains.Application.Progress;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.ControlFlow2.CSharp;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.ReSharper.Psi.Util;
  using JetBrains.Util;
  using Scopes;

  /// <summary>
  /// Defines the generate assignment check class.
  /// </summary>
  [SmartGenerate("Generate check if variable is null", "Generates statements that check for null or empty string.", Priority = 0)]
  public class AssignmentCheck : SmartGenerateBase
  {
    #region Protected methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">The get menu items parameters.</param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters)
    {
      IElement element = smartGenerateParameters.Element;
      List<ScopeEntry> scope = smartGenerateParameters.Scope;
      if (scope.Count == 0)
      {
        return;
      }

      ScopeEntry scopeEntry = scope[smartGenerateParameters.ScopeIndex];

      string name = scopeEntry.Name;
      IType type = scopeEntry.Type;

      CSharpControlFlowNullReferenceState state = GetExpressionNullReferenceState(smartGenerateParameters, element, name);
      if (state == CSharpControlFlowNullReferenceState.NOT_NULL || state == CSharpControlFlowNullReferenceState.NULL)
      {
        return;
      }

      TextRange range = StatementUtil.GetNewStatementPosition(element);

      if (type.GetPresentableName(element.Language) == "string")
      {
        AddMenuItem("Check if '{0}' is null or empty", "514313A0-91F4-4AE5-B4EB-2BB53736A023", range, name);
      }
      else
      {
        if (type.IsReferenceType())
        {
          AddMenuItem("Check if '{0}' is null", "F802DB32-A0B1-4227-BE5C-E7D20670284B", range, name);
        }
      }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Gets the state.
    /// </summary>
    /// <param name="smartGenerateParameters">The smart generate parameters.</param>
    /// <param name="element">The element.</param>
    /// <param name="name">The variable name.</param>
    /// <returns>The null reference state.</returns>
    private static CSharpControlFlowNullReferenceState GetExpressionNullReferenceState(SmartGenerateParameters smartGenerateParameters, IElement element, string name)
    {
      CSharpControlFlowNullReferenceState state = CSharpControlFlowNullReferenceState.UNKNOWN;

      PsiManager psiManager = PsiManager.GetInstance(smartGenerateParameters.Solution);
      using (ModificationCookie cookie = smartGenerateParameters.TextControl.Document.EnsureWritable())
      {
        if (cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS)
        {
          return state;
        }

        using (CommandCookie.Create(string.Format("Context Action {0}", "AssignmentCheck")))
        {
          psiManager.DoTransaction(delegate
          {
            state = GetExpressionNullReferenceState(element, name);

            throw new ProcessCancelledException();
          });
        }
      }

      return state;
    }

    /// <summary>
    /// Gets the state.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="name">The variable name.</param>
    /// <returns>The null reference state.</returns>
    private static CSharpControlFlowNullReferenceState GetExpressionNullReferenceState(IElement element, string name)
    {
      const CSharpControlFlowNullReferenceState UNKNOWN = CSharpControlFlowNullReferenceState.UNKNOWN;

      CSharpElementFactory factory = CSharpElementFactory.GetInstance(element.GetProject());

      IStatement statement = factory.CreateStatement("if(" + name + " == null) { }");
      IStatement anchorSatement = StatementUtil.GetPreviousStatement(element);

      IBlock block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if (block == null)
      {
        return UNKNOWN;
      }

      IIfStatement ifStatement = block.AddStatementAfter(statement, anchorSatement) as IIfStatement;
      if (ifStatement == null)
      {
        return UNKNOWN;
      }

      IEqualityExpression equalityExpression = ifStatement.Condition as IEqualityExpression;
      if (equalityExpression == null)
      {
        return UNKNOWN;
      }

      IReferenceExpression referenceExpression = equalityExpression.LeftOperand as IReferenceExpression;
      if (referenceExpression == null)
      {
        return UNKNOWN;
      }

      IFunctionDeclaration functionDeclaration = ifStatement.GetContainingElement<IFunctionDeclaration>(true);
      if (functionDeclaration == null)
      {
        return UNKNOWN;
      }

      ICSharpControlFlowGraf graf = CSharpControlFlowBuilder.Build(functionDeclaration);

      ICSharpControlFlowAnalysisResult inspect = graf.Inspect(true);

      return inspect.GetExpressionNullReferenceState(referenceExpression);
    }

    #endregion
  }
}