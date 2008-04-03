using System.Collections.Generic;
using AgentJohnson;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.Strings {
  /// <summary>
  /// 
  /// </summary>
  public class StringEmptyAnalyzer : ITokenTypeAnalyzer {
    #region Fields

    readonly ISolution _solution;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StringEmptyAnalyzer"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    public StringEmptyAnalyzer(ISolution solution) {
      _solution = solution;
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the solution.
    /// </summary>
    /// <value>The solution.</value>
    public ISolution Solution {
      get {
        return _solution;
      }
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

      List<SuggestionBase> suggestions = new List<SuggestionBase>();

      suggestions.Add(new StringEmptySuggestion(_solution, node));

      return suggestions.ToArray();
    }

    #endregion
  }
}