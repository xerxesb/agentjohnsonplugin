using System.Collections.Generic;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// 
  /// </summary>
  public interface ISmartGenerate {
    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The menu items.</returns>
    /// <value>The items.</value>
    IEnumerable<ISmartGenerateMenuItem> GetMenuItems(SmartGenerateParameters parameters);

    #endregion
  }
}