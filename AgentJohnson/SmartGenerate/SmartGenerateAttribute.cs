using System;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// 
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  public sealed class SmartGenerateAttribute : Attribute {
    #region Fields

    readonly string _description;
    readonly string _name;
    int _priority;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="SmartGenerateAttribute"/> class.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <param name="name">The name.</param>
    public SmartGenerateAttribute(string name, string description) {
      _description = description;
      _name = name;
    }

    #region Public properties

    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description {
      get {
        return _description ?? string.Empty;
      }
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name {
      get {
        return _name ?? string.Empty;
      }
    }

    /// <summary>
    /// Gets or sets the priority.
    /// </summary>
    /// <value>The priority.</value>
    public int Priority {
      get {
        return _priority;
      }
      set {
        _priority = value;
      }
    }

    #endregion
  }
}