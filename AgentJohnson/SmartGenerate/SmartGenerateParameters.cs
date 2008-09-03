using System.Collections.Generic;
using AgentJohnson.SmartGenerate.Scopes;
using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// Defines the get menu items parameters class.
  /// </summary>
  public class SmartGenerateParameters {
    #region Fields

    /// <summary>
    /// 
    /// </summary>
    public List<ScopeEntry> Scope;

    #endregion

    #region Public properties

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

    #endregion
  }
}