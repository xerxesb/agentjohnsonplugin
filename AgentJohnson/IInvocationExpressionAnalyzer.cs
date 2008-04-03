using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson {
  /// <summary>
  /// Declaration analyzers should implement this.
  /// </summary>
  public interface IInvocationExpressionAnalyzer {
    #region Public methods

    /// <summary>
    /// Analyzes the specified statement.
    /// </summary>
    /// <param name="invocationExpression">The invocation expression.</param>
    /// <returns></returns>
    SuggestionBase[] Analyze(IInvocationExpression invocationExpression);

    #endregion
  }
}