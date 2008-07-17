using System;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// 
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
  public sealed class SmartGenerateAttribute : Attribute {
    #region Fields

    string _description = string.Empty;
    string _name = string.Empty;
    int _priority;

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description {
      get {
        return _description ?? string.Empty;
      }
      set {
        _description = value;
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
      set {
        _name = value;
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