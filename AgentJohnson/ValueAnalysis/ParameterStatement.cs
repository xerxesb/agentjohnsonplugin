using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace AgentJohnson.ValueAnalysis {
  /// <summary>
  /// 
  /// </summary>
  class ParameterStatement {
    #region Fields

    IParameter _parameter;
    IStatement _statement;
    IAttributeInstance _attributeInstance;
    bool _nullable;
    bool _needsStatement = true;

    #endregion

    #region Public properties

    /// <summary>
    /// Gets or sets the attribute.
    /// </summary>
    /// <value>The attribute.</value>
    public IAttributeInstance AttributeInstance {
      get {
        return _attributeInstance;
      }
      set {
        _attributeInstance = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [needs statement].
    /// </summary>
    /// <value><c>true</c> if [needs statement]; otherwise, <c>false</c>.</value>
    public bool NeedsStatement {
      get {
        return _needsStatement;
      }
      set {
        _needsStatement = value;
      }
    }

    /// <summary>
    /// Gets or sets the parameter.
    /// </summary>
    /// <value>The parameter.</value>
    public IParameter Parameter {
      get {
        return _parameter;
      }
      set {
        _parameter = value;
      }
    }

    /// <summary>
    /// Gets or sets the statement.
    /// </summary>
    /// <value>The statement.</value>
    public IStatement Statement {
      get {
        return _statement;
      }
      set {
        _statement = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="ParameterStatement"/> is nullable.
    /// </summary>
    /// <value><c>true</c> if nullable; otherwise, <c>false</c>.</value>
    public bool Nullable {
      get {
        return _nullable;
      }
      set {
        _nullable = value;
      }
    }

    #endregion
    
  }
}