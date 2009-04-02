using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.Strings {
  /// <summary>
  /// 
  /// </summary>
  public class StringEmptyAnalyzer : ITokenTypeAnalyzer {
    #region Fields

    /// <summary>
    /// The current solution.
    /// </summary>
    readonly ISolution solution;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StringEmptyAnalyzer"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    public StringEmptyAnalyzer(ISolution solution) {
      this.solution = solution;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Analyzes the specified statement.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns></returns>
    public SuggestionBase[] Analyze(ITokenNode node) {
      TokenNodeType type = node.GetTokenType();
      if(!type.IsStringLiteral) {
        return null;
      }

      if(node.Language.Name != "CSHARP") {
        return null;
      }

      if(node.GetText() != "\"\"") {
        return null;
      }

      ITreeNode parent = node.Parent;

      if (parent.Parent is ISwitchLabelStatement)
      {
        return null;
      }

      IAttribute attribute = node.GetContainingElement(typeof(IAttribute), true) as IAttribute;
      if(attribute != null) {
        return null;
      }

      List<SuggestionBase> suggestions = new List<SuggestionBase>
      {
        new StringEmptySuggestion(this.solution, node)
      };

      return suggestions.ToArray();
    }

    #endregion
  }
}