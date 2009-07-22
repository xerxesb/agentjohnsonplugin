// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueAnalysisAnalyzer.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the ValueAnalysisAnalyzer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.ValueAnalysis
{
  using System.Collections.Generic;
  using Strings;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// The value analysis analyzer.
  /// </summary>
  public class ValueAnalysisAnalyzer : ITypeMemberDeclarationAnalyzer
  {
    #region Constants and Fields

    /// <summary>
    /// The _solution.
    /// </summary>
    private readonly ISolution _solution;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueAnalysisAnalyzer"/> class. 
    /// Initializes a new instance of the <see cref="StringEmptyAnalyzer"/> class.
    /// </summary>
    /// <param name="solution">
    /// The solution.
    /// </param>
    public ValueAnalysisAnalyzer(ISolution solution)
    {
      this._solution = solution;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the solution.
    /// </summary>
    /// <value>The solution.</value>
    public ISolution Solution
    {
      get
      {
        return this._solution;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region ITypeMemberDeclarationAnalyzer

    /// <summary>
    /// Analyzes the specified statement.
    /// </summary>
    /// <param name="typeMemberDeclaration">
    /// The type member declaration.
    /// </param>
    /// <returns>
    /// Returns the suggestion base[].
    /// </returns>
    public SuggestionBase[] Analyze(ITypeMemberDeclaration typeMemberDeclaration)
    {
      var modifiersOwnerDeclaration = typeMemberDeclaration as IModifiersOwnerDeclaration;
      if (modifiersOwnerDeclaration == null)
      {
        return null;
      }

      var valueAnalysisRefactoring = new ValueAnalysisRefactoring(typeMemberDeclaration, null);

      if (!valueAnalysisRefactoring.IsAvailable())
      {
        return null;
      }

      var suggestions = new List<SuggestionBase>();

      suggestions.Add(new ValueAnalysisSuggestion(this._solution, typeMemberDeclaration));

      return suggestions.ToArray();
    }

    #endregion

    #endregion
  }
}