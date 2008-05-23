using System;
using System.IO;
using Sitecore.Annotations;
using Sitecore.Diagnostics;

namespace AgentJohnson.Test {

  internal interface ITest {
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    string GetName(string Pipper);
  }

  internal abstract class AbstractClass {
    /// <summary>
    /// Gets or sets the X.
    /// </summary>
    /// <value>The X.</value>
    public abstract string X {
      get;
      set;
    }    
  } 

  /// <summary>
  /// 
  /// </summary>
  internal class TestClass {
    public enum TestEnum {
      Public,
      Protected,
      Private
    }

    public string AutoProp { get; set; }

    #region Public methods

    /// <summary>
    /// Throws the exception.
    /// </summary>
    public void ThrowException() {
      File.Delete("c:\autoexec.bat");

      throw new InvalidOperationException("Fail!");
    }

    /// <summary>
    /// Values the analysis.
    /// </summary>
    /// <param name="test">The test.</param>
    /// <returns>The analysis.</returns>
    [NotNull]
    public string ValueAnalysis(params string[] test) {
      TestEnum e = TestEnum.Public;

      string v = null;

      return v;
    }

    /// <summary>
    /// Values the analysis.
    /// </summary>
    /// <returns>The analysis.</returns>
    public string ValueAnalysis() {
      string result = ValueAnalysis(null);
      
      return result;
    }

    #endregion
  }

  /// <summary>
  /// 
  /// </summary>
  public class BaseClass {
    /// <summary>
    /// Gets the string.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>The string.</returns>
    public virtual string GetString([NotNull] string result) {
      Assert.ArgumentNotNull(result, "result");
      
      return result;
    }
  }

  public class OverloadedClass : BaseClass {
    /// <summary>
    /// Gets the string.
    /// </summary>
    /// <param name="result"></param>
    /// <returns>The string.</returns>
    [CanBeNull]
    public override string GetString([CanBeNull] string result) {
      return result;
    }
  }
}