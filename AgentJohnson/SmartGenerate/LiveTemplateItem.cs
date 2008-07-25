using System.Collections.Generic;
using JetBrains.Annotations;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// 
  /// </summary>
  public class LiveTemplateItem {
    #region Fields

    readonly Dictionary<string, string> _variables = new Dictionary<string, string>();

    #endregion

    #region Public properties

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    [CanBeNull]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    [CanBeNull]
    public string MenuText { get; set; }

    /// <summary>
    /// Gets or sets the name of the template.
    /// </summary>
    /// <value>The name of the template.</value>
    [CanBeNull]
    public string Shortcut { get; set; }

    /// <summary>
    /// Gets the variables.
    /// </summary>
    /// <value>The variables.</value>
    [NotNull]
    public Dictionary<string, string> Variables {
      get {
        return _variables;
      }
    }

    #endregion
  }
}