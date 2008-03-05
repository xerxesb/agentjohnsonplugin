using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson {
  /// <summary>
  /// Declaration analyzers should implement this.
  /// </summary>
  public interface IDeclarationAnalyzer {
    #region Public methods

    SuggestionBase[] Analyze(IDeclaration declaration);

    #endregion
  }
}