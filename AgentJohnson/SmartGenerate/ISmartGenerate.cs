using System.Collections.Generic;
using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// 
  /// </summary>
  public interface ISmartGenerate {
    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>The items.</value>
    IEnumerable<ISmartGenerateMenuItem> GetMenuItems(ISolution solution, IDataContext context, IElement element);

    #endregion
  }
}