namespace AgentJohnson.ValueAnalysis
{
  using System.Collections.Generic;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.ControlFlow2.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Resolve;
  using JetBrains.ReSharper.Psi.Util;

  /// <summary>
  /// 
  /// </summary>
  public class ReturnAnalyzer : IStatementAnalyzer
  {
    #region Fields

    private readonly ISolution solution;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ReturnAnalyzer"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    public ReturnAnalyzer(ISolution solution)
    {
      this.solution = solution;
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the solution.
    /// </summary>
    /// <value>The solution.</value>
    private ISolution Solution
    {
      get
      {
        return this.solution;
      }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Analyzes the specified statement.
    /// </summary>
    /// <param name="statement">The statement.</param>
    /// <returns></returns>
    public SuggestionBase[] Analyze(IStatement statement)
    {
      List<SuggestionBase> suggestions = new List<SuggestionBase>();

      IReturnStatement returnStatement = statement as IReturnStatement;
      if (returnStatement != null)
      {
        suggestions.AddRange(this.AnalyzeReturnStatement(returnStatement));
      }

      return suggestions.ToArray();
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Gets the value analysis.
    /// </summary>
    /// <param name="returnStatement">The return statement.</param>
    /// <param name="function">The function.</param>
    /// <returns>Returns the boolean.</returns>
    private static bool GetValueAnalysis(IReturnStatement returnStatement, IFunction function)
    {
      IReferenceExpression referenceExpression = returnStatement.Value as IReferenceExpression;
      if (referenceExpression == null)
      {
        return false;
      }

      ICSharpFunctionDeclaration functionDeclaration = function as ICSharpFunctionDeclaration;

      ICSharpControlFlowGraf graf = CSharpControlFlowBuilder.Build(functionDeclaration);

      graf.Inspect(true);

      ICSharpControlFlowAnalysisResult inspect = graf.Inspect(true);

      CSharpControlFlowNullReferenceState state = inspect.GetExpressionNullReferenceState(referenceExpression);

      switch (state)
      {
        case CSharpControlFlowNullReferenceState.UNKNOWN:
          return true;
        case CSharpControlFlowNullReferenceState.NOT_NULL:
          return false;
        case CSharpControlFlowNullReferenceState.NULL:
          return true;
        case CSharpControlFlowNullReferenceState.MAY_BE_NULL:
          return true;
      }

      return true;
    }

    /// <summary>
    /// Analyzes the return statement.
    /// </summary>
    /// <param name="returnStatement">The return statement.</param>
    /// <returns></returns>
    private IEnumerable<SuggestionBase> AnalyzeReturnStatement(IReturnStatement returnStatement)
    {
      List<SuggestionBase> suggestions = new List<SuggestionBase>();

      if (returnStatement.Value == null)
      {
        return suggestions;
      }

      if (this.RequiresAssertion(returnStatement))
      {
        suggestions.Add(new ReturnWarning(this.solution, returnStatement));
      }

      return suggestions;
    }

    /// <summary>
    /// Gets the is asserted.
    /// </summary>
    /// <param name="returnStatement">The return statement.</param>
    /// <returns>Returns the boolean.</returns>
    private bool GetIsAsserted(IReturnStatement returnStatement)
    {
      IInvocationExpression invocationExpression = returnStatement.Value as IInvocationExpression;
      if (invocationExpression == null)
      {
        return false;
      }

      IReferenceExpression invokedExpression = invocationExpression.InvokedExpression as IReferenceExpression;
      if (invokedExpression == null)
      {
        return false;
      }

      IResolveResult resolveResult = invokedExpression.Reference.Resolve();

      IMethod method = null;

      IMethodDeclaration methodDeclaration = resolveResult.DeclaredElement as IMethodDeclaration;
      if (methodDeclaration != null)
      {
        method = methodDeclaration as IMethod;
      }

      if (method == null)
      {
        method = resolveResult.DeclaredElement as IMethod;
      }

      if (method == null)
      {
        return false;
      }

      CodeAnnotationsCache codeAnnotationsCache = CodeAnnotationsCache.GetInstance(this.solution);

      return codeAnnotationsCache.IsAssertionMethod(method);
    }

    /// <summary>
    /// Determines whether this instance is asserted.
    /// </summary>
    /// <param name="returnStatement">The return statement.</param>
    /// <returns>
    /// 	<c>true</c> if this instance is asserted; otherwise, <c>false</c>.
    /// </returns>
    private bool RequiresAssertion(IReturnStatement returnStatement)
    {
      string canBeNullName = CodeAnnotationsCache.CanBeNullAttributeShortName;
      if (string.IsNullOrEmpty(canBeNullName))
      {
        return false;
      }

      string notNullName = CodeAnnotationsCache.NotNullAttributeShortName;
      if (string.IsNullOrEmpty(notNullName))
      {
        return false;
      }

      if (returnStatement.Value.IsConstantValue())
      {
        return false;
      }

      string returnValue = returnStatement.Value.GetText();
      if (returnValue == "string.Empty" || returnValue == "String.Empty" || returnValue == "null")
      {
        return false;
      }

      ICreationExpression creationExpression = returnStatement.Value as ICreationExpression;
      if (creationExpression != null)
      {
        return false;
      }

      IFunction function = returnStatement.GetContainingTypeMemberDeclaration() as IFunction;
      if (function == null)
      {
        return false;
      }

      IType type = function.ReturnType;
      if (!type.IsReferenceType())
      {
        return false;
      }

      if (HasAnnotation(function))
      {
        return false;
      }

      /*
      string canBeNullValueAttributeTypeName = CodeAnnotationsCache.GetInstance(this.Solution)..GetAttributeTypeForElement(returnStatement, canBeNullName);
      CLRTypeName canBeNullTypeName = new CLRTypeName(canBeNullValueAttributeTypeName);
      IList<IAttributeInstance> instances = function.GetAttributeInstances(canBeNullTypeName, true);
      if (instances != null && instances.Count > 0)
      {
        return false;
      }

      string notNullValueAttributeTypeName = CodeAnnotationsCache.GetInstance(this.Solution).GetAttributeTypeForElement(notNullName);
      CLRTypeName notNullTypeName = new CLRTypeName(notNullValueAttributeTypeName);
      instances = function.GetAttributeInstances(notNullTypeName, true);
      if (instances == null || instances.Count == 0)
      {
        return false;
      }
      */

      Rule rule = Rule.GetRule(type, function.Language) ?? Rule.GetDefaultRule();
      if (rule == null)
      {
        return false;
      }

      if (string.IsNullOrEmpty(rule.ReturnAssertion))
      {
        return false;
      }

      bool isAsserted = this.GetIsAsserted(returnStatement);
      if (isAsserted)
      {
        return false;
      }

      return GetValueAnalysis(returnStatement, function);
    }

    /// <summary>
    /// Determines whether this instance has annotation.
    /// </summary>
    /// <param name="function">The function.</param>
    /// <returns>
    /// 	<c>true</c> if this instance has annotation; otherwise, <c>false</c>.
    /// </returns>
    private bool HasAnnotation(IFunction function)
    {
      CodeAnnotationsCache codeAnnotationsCache = CodeAnnotationsCache.GetInstance(this.Solution);

      IList<IAttributeInstance> instances = function.GetAttributeInstances(true);
      foreach (IAttributeInstance list in instances)
      {
        if (codeAnnotationsCache.IsAnnotationAttribute(list))
        {
          return true;
        }
      }

      return false;
    }

    #endregion
  }
}