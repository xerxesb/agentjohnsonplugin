namespace TestProject {
  using System;

  /// <summary>
  /// 
  /// </summary>
  public class NegateIfStatement {
    #region Public methods

    /// <summary>
    /// Tests this instance.
    /// </summary>
    public void Test() {
      int v = GetInt();

      if(v > v + 2) {
        return;
      }

      if (v == 2) {
        return;
      } 

      if(GetBool()) {
        return;
      }

      if(true) {
        return;
      }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Gets the boolean.
    /// </summary>
    /// <returns></returns>
    static bool GetBool() {
      return false;
    }

    /// <summary>
    /// Gets the integer.
    /// </summary>
    /// <returns></returns>
    static int GetInt() {
      return 0;
    }

    #endregion
  }
}