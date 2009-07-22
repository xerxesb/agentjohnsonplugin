// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeMemberDeclarationAnalyzer.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Function declaration analyzers should implement this.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson
{
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Function declaration analyzers should implement this.
  /// </summary>
  public interface ITypeMemberDeclarationAnalyzer
  {
    #region Public Methods

    /// <summary>
    /// The analyze.
    /// </summary>
    /// <param name="typeMemberDeclaration">
    /// The type member declaration.
    /// </param>
    /// <returns>
    /// </returns>
    SuggestionBase[] Analyze(ITypeMemberDeclaration typeMemberDeclaration);

    #endregion
  }
}