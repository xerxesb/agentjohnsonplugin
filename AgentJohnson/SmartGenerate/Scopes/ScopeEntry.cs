using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.Scopes {
  /// <summary>
  /// Defines the scope entry class.
  /// </summary>
  public class ScopeEntry {
    #region Public properties

    /// <summary>
    /// Gets or sets the element.
    /// </summary>
    /// <value>The element.</value>
    public IElement Element { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is assigned.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is assigned; otherwise, <c>false</c>.
    /// </value>
    public bool IsAssigned { get; set; }
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    public IType Type { get; set; }

    #endregion
  }
}