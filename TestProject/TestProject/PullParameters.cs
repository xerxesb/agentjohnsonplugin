namespace TestProject {
  /// <summary>
  /// 
  /// </summary>
  public class PullParameters {
    #region Public methods

    /// <summary>
    /// Pulls the specified text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="escape">if set to <c>true</c> [escape].</param>
    public void Pull(string text, int offset, bool escape) {
      Pulled();
    }

    /// <summary>
    /// Lives the macro.
    /// </summary>
    public void LiveMacro(string text, int offset, bool escape) {
      this.Pull();
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Pulleds this instance.
    /// </summary>
    void Pulled() {
    }

    #endregion
  }
}