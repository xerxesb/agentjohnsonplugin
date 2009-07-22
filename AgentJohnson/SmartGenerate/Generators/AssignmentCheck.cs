// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssignmentCheck.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the generate assignment check class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate.Generators
{
  using JetBrains.Application;
  using JetBrains.Application.Progress;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.ControlFlow2.CSharp;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.ReSharper.Psi.Util;
  using JetBrains.Util;

  /// <summary>
  /// Defines the generate assignment check class.
  /// </summary>
  [SmartGenerate("Generate check if variable is null", "Generates statements that check for null or empty string.", Priority = 0)]
  public class AssignmentCheck : SmartGenerateHandlerBase
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
      var scope = smartGenerateParameters.Scope;
      if (scope.Count == 0)
      {
        return;
      }

      var scopeEntry = scope[smartGenerateParameters.ScopeIndex];

      var name = scopeEntry.Name;
      var type = scopeEntry.Type;

      var state = GetExpressionNullReferenceState(smartGenerateParameters, element, name);
      if (state == CSharpControlFlowNullReferenceState.NOT_NULL || state == CSharpControlFlowNullReferenceState.NULL)
      {
        return;
      }

      var range = StatementUtil.GetNewStatementPosition(element);

      if (type.GetPresentableName(element.Language) == "string")
      {
        this.AddAction("Check if '{0}' is null or empty", "514313A0-91F4-4AE5-B4EB-2BB53736A023", range, name);
      }
      else
      {
        if (type.IsReferenceType())
        {
          this.AddAction("Check if '{0}' is null", "F802DB32-A0B1-4227-BE5C-E7D20670284B", range, name);
        }
      }
    }

    /// <summary>
    /// Gets the state.
    /// </summary>
    /// <param name="smartGenerateParameters">
    /// The smart generate parameters.
    /// </param>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <param name="name">
    /// The variable name.
    /// </param>
    /// <returns>
    /// The null reference state.
    /// </returns>
    private static CSharpControlFlowNullReferenceState GetExpressionNullReferenceState(SmartGenerateParameters smartGenerateParameters, IElement element, string name)
    {
      var state = CSharpControlFlowNullReferenceState.UNKNOWN;

      var psiManager = PsiManager.GetInstance(smartGenerateParameters.Solution);
      using (var cookie = smartGenerateParameters.TextControl.Document.EnsureWritable())
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
    /// <param name="element">
    /// The element.
    /// </param>
    /// <param name="name">
    /// The variable name.
    /// </param>
    /// <returns>
    /// The null reference state.
    /// </returns>
    private static CSharpControlFlowNullReferenceState GetExpressionNullReferenceState(IElement element, string name)
    {
      const CSharpControlFlowNullReferenceState UNKNOWN = CSharpControlFlowNullReferenceState.UNKNOWN;

      var factory = CSharpElementFactory.GetInstance(element.GetPsiModule());

      var statement = factory.CreateStatement("if(" + name + " == null) { }");
      var anchorSatement = StatementUtil.GetPreviousStatement(element);

      var block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if (block == null)
      {
        return UNKNOWN;
      }

      var ifStatement = block.AddStatementAfter(statement, anchorSatement) as IIfStatement;
      if (ifStatement == null)
      {
        return UNKNOWN;
      }

      var equalityExpression = ifStatement.Condition as IEqualityExpression;
      if (equalityExpression == null)
      {
        return UNKNOWN;
      }

      var referenceExpression = equalityExpression.LeftOperand as IReferenceExpression;
      if (referenceExpression == null)
      {
        return UNKNOWN;
      }

      var functionDeclaration = ifStatement.GetContainingElement<ICSharpFunctionDeclaration>(true);
      if (functionDeclaration == null)
      {
        return UNKNOWN;
      }

      var graf = CSharpControlFlowBuilder.Build(functionDeclaration);

      var inspect = graf.Inspect(true);

      return inspect.GetExpressionNullReferenceState(referenceExpression);
    }

    #endregion
  }
}