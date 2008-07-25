namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// 
  /// </summary>
  internal class SmartGenerateInfo {
    #region Public properties

    public string Description { get; set; }
    public ISmartGenerate Handler { get; set; }
    public string Name { get; set; }
    public int Priority { get; set; }

    #endregion

    #region Public methods

    /// <summary>
    /// Returns the fully qualified type name of this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"/> containing a fully qualified type name.
    /// </returns>
    public override string ToString() {
      return Name;
    }

    #endregion
  }
}