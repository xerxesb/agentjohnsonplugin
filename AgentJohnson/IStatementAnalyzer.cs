// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStatementAnalyzer.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Declaration analyzers should implement this.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson
{
  using JetBrains.ReSharper.Psi.CSharp.Tree;

  /// <summary>
  /// Declaration analyzers should implement this.
  /// </summary>
  public interface IStatementAnalyzer
  {
    #region Public Methods

    /// <summary>
    /// The analyze.
    /// </summary>
    /// <param name="statement">
    /// The statement.
    /// </param>
    /// <returns>
    /// </returns>
    SuggestionBase[] Analyze(IStatement statement);

    #endregion
  }
}