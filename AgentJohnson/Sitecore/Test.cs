using System;
using System.IO;
using JetBrains.Util;

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

      throw new InvalidOperationException("");
    }

    /// <summary>
    /// Values the analysis.
    /// </summary>
    /// <param name="pipper">The pipper.</param>
    public void ValueAnalysis([Nullable] string pipper) {
    }

    #endregion
  }
}