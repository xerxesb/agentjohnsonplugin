// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITokenAnalyzer.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Declaration analyzers should implement this.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson
{
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Declaration analyzers should implement this.
  /// </summary>
  public interface ITokenTypeAnalyzer
  {
    #region Public Methods

    /// <summary>
    /// Analyzes the specified statement.
    /// </summary>
    /// <param name="node">
    /// The node.
    /// </param>
    /// <returns>
    /// </returns>
    SuggestionBase[] Analyze(ITokenNode node);

    #endregion
  }
}