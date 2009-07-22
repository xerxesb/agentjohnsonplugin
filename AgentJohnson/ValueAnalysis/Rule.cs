// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Rule.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The rule.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.ValueAnalysis
{
  using System.Collections.Generic;
  using JetBrains.ReSharper.Psi;

  /// <summary>
  /// The rule.
  /// </summary>
  public class Rule
  {
    #region Constants and Fields

    /// <summary>
    /// The _can be null.
    /// </summary>
    private bool _canBeNull;

    /// <summary>
    /// The _non public parameter assertion.
    /// </summary>
    private string _nonPublicParameterAssertion;

    /// <summary>
    /// The _not null.
    /// </summary>
    private bool _notNull;

    /// <summary>
    /// The _public parameter assertion.
    /// </summary>
    private string _publicParameterAssertion;

    /// <summary>
    /// The _return assertion.
    /// </summary>
    private string _returnAssertion;

    /// <summary>
    /// The _type name.
    /// </summary>
    private string _typeName;

    /// <summary>
    /// The _value assertions.
    /// </summary>
    private List<string> _valueAssertions = new List<string>();

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets a value indicating whether this instance can be null.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance can be null; otherwise, <c>false</c>.
    /// </value>
    public bool CanBeNull
    {
      get
      {
        return this._canBeNull;
      }

      set
      {
        this._canBeNull = value;
      }
    }

    /// <summary>
    /// Gets or sets the non public parameters assertion.
    /// </summary>
    /// <value>The non public parameters assertion.</value>
    public string NonPublicParameterAssertion
    {
      get
      {
        return this._nonPublicParameterAssertion ?? string.Empty;
      }

      set
      {
        this._nonPublicParameterAssertion = value;
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [not null].
    /// </summary>
    /// <value><c>true</c> if [not null]; otherwise, <c>false</c>.</value>
    public bool NotNull
    {
      get
      {
        return this._notNull;
      }

      set
      {
        this._notNull = value;
      }
    }

    /// <summary>
    /// Gets or sets the public parameter assertion.
    /// </summary>
    /// <value>The public parameter assertion.</value>
    public string PublicParameterAssertion
    {
      get
      {
        return this._publicParameterAssertion ?? string.Empty;
      }

      set
      {
        this._publicParameterAssertion = value;
      }
    }

    /// <summary>
    /// Gets or sets the return assertion.
    /// </summary>
    /// <value>The return assertion.</value>
    public string ReturnAssertion
    {
      get
      {
        return this._returnAssertion ?? string.Empty;
      }

      set
      {
        this._returnAssertion = value;
      }
    }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    public string TypeName
    {
      get
      {
        return this._typeName ?? string.Empty;
      }

      set
      {
        this._typeName = value;
      }
    }

    /// <summary>
    /// Gets or sets the value assertions.
    /// </summary>
    /// <value>The value assertions.</value>
    public List<string> ValueAssertions
    {
      get
      {
        return this._valueAssertions;
      }

      set
      {
        this._valueAssertions = value;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the default type configuration.
    /// </summary>
    /// <returns>
    /// The default type configuration.
    /// </returns>
    public static Rule GetDefaultRule()
    {
      var configurations = ValueAnalysisSettings.Instance.Rules;

      foreach (var configuration in configurations)
      {
        if (configuration.TypeName == "*" || configuration.TypeName == "System.Object")
        {
          return configuration;
        }
      }

      return null;
    }

    /// <summary>
    /// Gets the type configuration.
    /// </summary>
    /// <param name="type">
    /// The type.
    /// </param>
    /// <param name="languageType">
    /// Type of the language.
    /// </param>
    /// <returns>
    /// The type configuration.
    /// </returns>
    public static Rule GetRule(IType type, PsiLanguageType languageType)
    {
      string clrName = null;

      var arrayType = type as IArrayType;
      if (arrayType != null)
      {
        clrName = arrayType.GetLongPresentableName(languageType);
      }

      var declaredType = type as IDeclaredType;
      if (declaredType != null)
      {
        clrName = declaredType.GetCLRName();
      }

      if (string.IsNullOrEmpty(clrName))
      {
        return null;
      }

      var module = type.Module;
      if (module == null)
      {
        return null;
      }

      var configurations = ValueAnalysisSettings.Instance.Rules;

      foreach (var configuration in configurations)
      {
        if (configuration.TypeName == "*" || configuration.TypeName == "System.Object")
        {
          continue;
        }

        if (!string.IsNullOrEmpty(clrName) && clrName == configuration.TypeName)
        {
          return configuration;
        }
      }

      if (declaredType == null)
      {
        return null;
      }

      // check for subclass
      foreach (var configuration in configurations)
      {
        if (configuration.TypeName == "*" || configuration.TypeName == "System.Object")
        {
          continue;
        }

        var baseType = TypeFactory.CreateTypeByCLRName(configuration.TypeName, module);

        if (declaredType.IsSubtypeOf(baseType))
        {
          return configuration;
        }
      }

      return null;
    }

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns>
    /// </returns>
    public Rule Clone()
    {
      var result = new Rule();

      result.TypeName = this.TypeName;
      result.NotNull = this.NotNull;
      result.CanBeNull = this.CanBeNull;
      result.PublicParameterAssertion = this.PublicParameterAssertion;
      result.NonPublicParameterAssertion = this.NonPublicParameterAssertion;
      result.ReturnAssertion = this.ReturnAssertion;

      foreach (var valueAssertion in this.ValueAssertions)
      {
        result.ValueAssertions.Add(valueAssertion);
      }

      return result;
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
    /// </returns>
    /// <filterpriority>2</filterpriority>
    public override string ToString()
    {
      return this.TypeName ?? string.Empty;
    }

    #endregion
  }
}