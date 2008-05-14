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
    [NotNull]
    public string ValueAnalysis(string test) {
      Assert.ArgumentNotNull(test, "test");

      TestEnum e = TestEnum.Public;

      string v = "!";

      return v;
    }

    /// <summary>
    /// Values the analysis.
    /// </summary>
    public string ValueAnalysis() {
      string result = ValueAnalysis(null);
      
      return result;
    }

    #endregion
  }
}