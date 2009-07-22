// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringEmptyAnalyzer.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The string empty analyzer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Strings
{
  using System.Collections.Generic;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// The string empty analyzer.
  /// </summary>
  public class StringEmptyAnalyzer : ITokenTypeAnalyzer
  {
    #region Constants and Fields

    /// <summary>
    /// The current solution.
    /// </summary>
    private readonly ISolution solution;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StringEmptyAnalyzer"/> class.
    /// </summary>
    /// <param name="solution">
    /// The solution.
    /// </param>
    public StringEmptyAnalyzer(ISolution solution)
    {
      this.solution = solution;
    }

    #endregion

    #region Implemented Interfaces

    #region ITokenTypeAnalyzer

    /// <summary>
    /// Analyzes the specified statement.
    /// </summary>
    /// <param name="node">
    /// The node.
    /// </param>
    /// <returns>
    /// </returns>
    public SuggestionBase[] Analyze(ITokenNode node)
    {
      var type = node.GetTokenType();
      if (!type.IsStringLiteral)
      {
        return null;
      }

      if (node.Language.Name != "CSHARP")
      {
        return null;
      }

      if (node.GetText() != "\"\"")
      {
        return null;
      }

      var parent = node.Parent;

      if (parent.Parent is ISwitchLabelStatement)
      {
        return null;
      }

      var attribute = node.GetContainingElement(typeof(IAttribute), true) as IAttribute;
      if (attribute != null)
      {
        return null;
      }

      var suggestions = new List<SuggestionBase>
      {
        new StringEmptySuggestion(this.solution, node)
      };

      return suggestions.ToArray();
    }

    #endregion

    #endregion
  }
}