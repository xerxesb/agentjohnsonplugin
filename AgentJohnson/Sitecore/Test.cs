using System;
using System.IO;
using Sitecore.Annotations;

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
    public void ValueAnalysis([NotNull] string test) {
    }

    /// <summary>
    /// Values the analysis.
    /// </summary>
    public void ValueAnalysis() {
      ValueAnalysis(null);
    }

    #endregion
  }
}