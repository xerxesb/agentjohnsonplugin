using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace AgentJohnson {
  /// <summary>
  /// Function declaration analyzers should implement this.
  /// </summary>
  public interface IFunctionDeclarationAnalyzer {
    #region Public methods

    SuggestionBase[] Analyze(IFunctionDeclaration functionDeclaration);

    #endregion
  }
}