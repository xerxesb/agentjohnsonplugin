namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// Defines the smart generate handler data class.
  /// </summary>
  internal class SmartGenerateHandlerData {
    #region Public properties

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the handler.
    /// </summary>
    /// <value>The handler.</value>
    public ISmartGenerateHandler Handler { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the priority.
    /// </summary>
    /// <value>The priority.</value>
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