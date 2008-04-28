using System.Collections.Generic;
using AgentJohnson;
using AgentJohnson.Strings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.ValueAnalysis {
  /// <summary>
  /// 
  /// </summary>
  public class ValueAnalysisAnalyzer : ITypeMemberDeclarationAnalyzer {
    #region Fields

    readonly ISolution _solution;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StringEmptyAnalyzer"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    public ValueAnalysisAnalyzer(ISolution solution) {
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
    /// <param name="typeMemberDeclaration">The type member declaration.</param>
    /// <returns></returns>
    public SuggestionBase[] Analyze(ITypeMemberDeclaration typeMemberDeclaration) {
      IModifiersOwnerDeclaration modifiersOwnerDeclaration = typeMemberDeclaration as IModifiersOwnerDeclaration;
      if(modifiersOwnerDeclaration == null) {
        return null;
      }

      ValueAnalysisRefactoring valueAnalysisRefactoring = new ValueAnalysisRefactoring(typeMemberDeclaration, null);

      if(!valueAnalysisRefactoring.IsAvailable()) {
        return null;
      }

      List<SuggestionBase> suggestions = new List<SuggestionBase>();

      suggestions.Add(new ValueAnalysisSuggestion(_solution, typeMemberDeclaration));

      return suggestions.ToArray();
    }

    #endregion
  }
}