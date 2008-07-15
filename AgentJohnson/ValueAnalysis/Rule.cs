using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace AgentJohnson.ValueAnalysis {
  /// <summary>
  /// 
  /// </summary>
  public class Rule {
    #region Fields

    bool _canBeNull;
    string _nonPublicParameterAssertion;
    bool _notNull;
    string _publicParameterAssertion;
    string _returnAssertion;
    string _typeName;
    List<string> _valueAssertions = new List<string>();

    #endregion

    #region Public properties

    /// <summary>
    /// Gets or sets a value indicating whether this instance can be null.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance can be null; otherwise, <c>false</c>.
    /// </value>
    public bool CanBeNull {
      get {
        return _canBeNull;
      }
      set {
        _canBeNull = value;
      }
    }

    /// <summary>
    /// Gets or sets the non public parameters assertion.
    /// </summary>
    /// <value>The non public parameters assertion.</value>
    public string NonPublicParameterAssertion {
      get {
        return _nonPublicParameterAssertion ?? string.Empty;
      }
      set {
        _nonPublicParameterAssertion = value;
      }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [not null].
    /// </summary>
    /// <value><c>true</c> if [not null]; otherwise, <c>false</c>.</value>
    public bool NotNull {
      get {
        return _notNull;
      }
      set {
        _notNull = value;
      }
    }
    /// <summary>
    /// Gets or sets the public parameter assertion.
    /// </summary>
    /// <value>The public parameter assertion.</value>
    public string PublicParameterAssertion {
      get {
        return _publicParameterAssertion ?? string.Empty;
      }
      set {
        _publicParameterAssertion = value;
      }
    }

    /// <summary>
    /// Gets or sets the return assertion.
    /// </summary>
    /// <value>The return assertion.</value>
    public string ReturnAssertion {
      get {
        return _returnAssertion ?? string.Empty;
      }
      set {
        _returnAssertion = value;
      }
    }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    public string TypeName {
      get {
        return _typeName ?? string.Empty;
      }
      set {
        _typeName = value;
      }
    }
    /// <summary>
    /// Gets or sets the value assertions.
    /// </summary>
    /// <value>The value assertions.</value>
    public List<string> ValueAssertions {
      get {
        return _valueAssertions;
      }
      set {
        _valueAssertions = value;
      }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Clones this instance.
    /// </summary>
    /// <returns></returns>
    public Rule Clone() {
      Rule result = new Rule();

      result.TypeName = TypeName;
      result.NotNull = NotNull;
      result.CanBeNull = CanBeNull;
      result.PublicParameterAssertion = PublicParameterAssertion;
      result.NonPublicParameterAssertion = NonPublicParameterAssertion;
      result.ReturnAssertion = ReturnAssertion;

      foreach(string valueAssertion in ValueAssertions) {
        result.ValueAssertions.Add(valueAssertion);
      }

      return result;
    }

    /// <summary>
    /// Gets the default type configuration.
    /// </summary>
    /// <returns>The default type configuration.</returns>
    public static Rule GetDefaultRule() {
      List<Rule> configurations = ValueAnalysisSettings.Instance.Rules;

      foreach(Rule configuration in configurations) {
        if(configuration.TypeName == "*" || configuration.TypeName == "System.Object") {
          return configuration;
        }
      }

      return null;
    }

    /// <summary>
    /// Gets the type configuration.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="languageType">Type of the language.</param>
    /// <returns>The type configuration.</returns>
    public static Rule GetRule(IType type, PsiLanguageType languageType) {
      string clrName = null;

      IArrayType arrayType = type as IArrayType;
      if(arrayType != null) {
        clrName = arrayType.GetLongPresentableName(languageType);
      }

      IDeclaredType declaredType = type as IDeclaredType;
      if(declaredType != null) {
        clrName = declaredType.GetCLRName();
      }

      if(string.IsNullOrEmpty(clrName)) {
        return null;
      }

      IModule module = type.Module;
      if(module == null) {
        return null;
      }

      List<Rule> configurations = ValueAnalysisSettings.Instance.Rules;

      foreach(Rule configuration in configurations) {
        if(configuration.TypeName == "*" || configuration.TypeName == "System.Object") {
          continue;
        }

        if(!string.IsNullOrEmpty(clrName) && clrName == configuration.TypeName) {
          return configuration;
        }
      }

      if (declaredType == null) {
        return null;
      }

      // check for subclass
      foreach(Rule configuration in configurations) {
        if(configuration.TypeName == "*" || configuration.TypeName == "System.Object") {
          continue;
        }

        IDeclaredType baseType = TypeFactory.CreateTypeByCLRName(configuration.TypeName, module);

        if(declaredType.IsSubtypeOf(baseType)) {
          return configuration;
        }
      }

      return null;
    }

    ///<summary>
    ///Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    ///</summary>
    ///
    ///<returns>
    ///A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
    ///</returns>
    ///<filterpriority>2</filterpriority>
    public override string ToString() {
      return TypeName ?? string.Empty;
    }

    #endregion
  }
}