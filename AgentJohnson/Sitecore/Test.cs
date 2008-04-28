using System;
using System.Collections.Generic;
using System.IO;
using Sitecore.Annotations;
using Sitecore.Diagnostics;

namespace AgentJohnson.Test {
  /// <summary>
  /// 
  /// </summary>
  internal class TestClass {
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