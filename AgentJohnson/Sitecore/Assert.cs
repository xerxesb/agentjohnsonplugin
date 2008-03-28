using System;
using JetBrains.Annotations;
using Sitecore.Annotations;

namespace Sitecore.Annotations {
  /// <summary>
  /// 
  /// </summary>
  public class AllowNullAttribute: Attribute {
    #region Fields

    string _name;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AllowNullAttribute"/> class.
    /// </summary>
    public AllowNullAttribute() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AllowNullAttribute"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public AllowNullAttribute(string name) {
      _name = name;
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name {
      get {
        return _name;
      }
      set {
        _name = value;
      }
    }

    #endregion
  }

  /// <summary>
  /// 
  /// </summary>
  [AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class CanBeNullAttribute: Attribute {
  }

  /// <summary>
  /// 
  /// </summary>
  [AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class NotNullAttribute: Attribute {
  }
}

namespace Sitecore.Diagnostics {
  /// <summary>
  /// 
  /// </summary>
  public static class Assert {
    #region Public methods

    /// <summary>
    /// Arguments the not null.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <param name="name">The name.</param>
    [AssertionMethod]
    public static void ArgumentNotNull([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] object argument, string name) {
    }

    /// <summary>
    /// Arguments the not null or empty.
    /// </summary>
    /// <param name="argument">The argument.</param>
    /// <param name="name">The name.</param>
    [AssertionMethod]
    [AllowNull("argument")]
    public static void ArgumentNotNullOrEmpty([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] string argument, string name) {
    }

    #endregion
  }

  /// <summary>
  /// 
  /// </summary>
  public class Test {
    #region Fields

    [Annotations.NotNull]
    string _test;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Test"/> class.
    /// </summary>
    public Test() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Test"/> class.
    /// </summary>
    /// <param name="test">The test.</param>
    public Test([Annotations.NotNull] string test) {
      Assert.ArgumentNotNullOrEmpty(test, "test");

      _test = test;
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets or sets the test.
    /// </summary>
    /// <value>The test.</value>
    public string TestProperty {
      get {
        return _test;
      }
      set {
        _test = value;
      }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Gets the test.
    /// </summary>
    /// <returns></returns>
    public string GetTest() {
      return _test;
    }

    /// <summary>
    /// Sets the test.
    /// </summary>
    /// <param name="value">The value.</param>
    [Annotations.CanBeNull]
    public string SetTest([Annotations.NotNull] string value) {
      Assert.ArgumentNotNullOrEmpty(value, "value");

      _test = value;
      return null;
    }

    #endregion
  }

  /// <summary>
  /// 
  /// </summary>
  public static class Texts {
  }
}