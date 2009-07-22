// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterStatement.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The parameter statement.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.ValueAnalysis
{
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp.Tree;

  /// <summary>
  /// The parameter statement.
  /// </summary>
  internal class ParameterStatement
  {
    #region Constants and Fields

    /// <summary>
    /// The _attribute instance.
    /// </summary>
    private IAttributeInstance _attributeInstance;

    /// <summary>
    /// The _needs statement.
    /// </summary>
    private bool _needsStatement = true;

    /// <summary>
    /// The _nullable.
    /// </summary>
    private bool _nullable;

    /// <summary>
    /// The _parameter.
    /// </summary>
    private IParameter _parameter;

    /// <summary>
    /// The _statement.
    /// </summary>
    private IStatement _statement;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the attribute.
    /// </summary>
    /// <value>The attribute.</value>
    public IAttributeInstance AttributeInstance
    {
      get
      {
        return this._attributeInstance;
      }

      set
      {
        this._attributeInstance = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [needs statement].
    /// </summary>
    /// <value><c>true</c> if [needs statement]; otherwise, <c>false</c>.</value>
    public bool NeedsStatement
    {
      get
      {
        return this._needsStatement;
      }

      set
      {
        this._needsStatement = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="ParameterStatement"/> is nullable.
    /// </summary>
    /// <value><c>true</c> if nullable; otherwise, <c>false</c>.</value>
    public bool Nullable
    {
      get
      {
        return this._nullable;
      }

      set
      {
        this._nullable = value;
      }
    }

    /// <summary>
    /// Gets or sets the parameter.
    /// </summary>
    /// <value>The parameter.</value>
    public IParameter Parameter
    {
      get
      {
        return this._parameter;
      }

      set
      {
        this._parameter = value;
      }
    }

    /// <summary>
    /// Gets or sets the statement.
    /// </summary>
    /// <value>The statement.</value>
    public IStatement Statement
    {
      get
      {
        return this._statement;
      }

      set
      {
        this._statement = value;
      }
    }

    #endregion
  }
}