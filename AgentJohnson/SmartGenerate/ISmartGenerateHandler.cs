using System.Collections.Generic;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// 
  /// </summary>
  public interface ISmartGenerateHandler {
    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The menu items.</returns>
    /// <value>The items.</value>
    IEnumerable<ISmartGenerateAction> GetMenuItems(SmartGenerateParameters parameters);

    #endregion
  }
}