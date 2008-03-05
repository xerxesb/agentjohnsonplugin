using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson {
  /// <summary>
  /// Function declaration analyzers should implement this.
  /// </summary>
  public interface ITypeMemberDeclarationAnalyzer {
    #region Public methods

    SuggestionBase[] Analyze(ITypeMemberDeclaration typeMemberDeclaration);

    #endregion
  }
}