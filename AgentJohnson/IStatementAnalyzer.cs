using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace AgentJohnson {
  /// <summary>
  /// Declaration analyzers should implement this.
  /// </summary>
  public interface IStatementAnalyzer {
    #region Public methods

    SuggestionBase[] Analyze(IStatement statement);

    #endregion
  }
}