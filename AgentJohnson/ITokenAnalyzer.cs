using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson {
  /// <summary>
  /// Declaration analyzers should implement this.
  /// </summary>
  public interface ITokenTypeAnalyzer {
    #region Public methods

    /// <summary>
    /// Analyzes the specified statement.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns></returns>
    SuggestionBase[] Analyze(ITokenNode node);

    #endregion
  }
}
