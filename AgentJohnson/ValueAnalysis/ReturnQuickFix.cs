using System.Collections.Generic;
using JetBrains.ReSharper.Daemon;
using JetBrains.Util;

namespace AgentJohnson.ValueAnalysis {
  /// <summary>
  /// 
  /// </summary>
  [QuickFix]
  public class ReturnQuickFix : IQuickFix {
    #region Fields

    readonly ReturnWarning _warning;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ReturnQuickFix"/> class.
    /// </summary>
    /// <param name="warning">The suggestion.</param>
    public ReturnQuickFix(ReturnWarning warning) {
      _warning = warning;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Check if this action is available at the constructed context.
    /// Actions could store precalculated info in <paramref name="cache"/> to share it between different actions
    /// </summary>
    /// <param name="cache"></param>
    /// <returns></returns>
    public bool IsAvailable(IUserDataHolder cache) {
      return true;
    }

    #endregion

    #region IQuickFix Members

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>The items.</value>
    public IBulbItem[] Items {
      get {
        List<IBulbItem> items = new List<IBulbItem>();

        items.Add(new ReturnBulbItem(_warning));

        return items.ToArray();
      }
    }

    #endregion
  }
}