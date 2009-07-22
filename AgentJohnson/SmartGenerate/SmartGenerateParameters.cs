// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmartGenerateParameters.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the get menu items parameters class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate
{
  using System.Collections.Generic;
  using Scopes;
  using JetBrains.ActionManagement;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.TextControl;

  /// <summary>
  /// Defines the get menu items parameters class.
  /// </summary>
  public class SmartGenerateParameters
  {
    #region Constants and Fields

    /// <summary>
    /// The scope.
    /// </summary>
    public List<ScopeEntry> Scope;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the context.
    /// </summary>
    /// <value>The context.</value>
    public IDataContext Context { get; set; }

    /// <summary>
    /// Gets or sets the element.
    /// </summary>
    /// <value>The element.</value>
    public IElement Element { get; set; }

    /// <summary>
    /// Gets or sets the previous statement.
    /// </summary>
    /// <value>The previous statement.</value>
    public IStatement PreviousStatement { get; set; }

    /// <summary>
    /// Gets or sets the index.
    /// </summary>
    /// <value>The index.</value>
    public int ScopeIndex { get; set; }

    /// <summary>
    /// Gets or sets the solution.
    /// </summary>
    /// <value>The solution.</value>
    public ISolution Solution { get; set; }

    /// <summary>
    /// Gets or sets the text control.
    /// </summary>
    /// <value>The text control.</value>
    public ITextControl TextControl { get; set; }

    #endregion
  }
}