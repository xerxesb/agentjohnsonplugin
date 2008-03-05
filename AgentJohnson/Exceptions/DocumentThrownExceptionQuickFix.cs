using System.Collections.Generic;
using JetBrains.ReSharper.Daemon;
using JetBrains.Util;

namespace AgentJohnson.Exceptions {
  /// <summary>
  /// 
  /// </summary>
  [QuickFix]
  public class DocumentThrownExceptionQuickFix : IQuickFix {
    #region Fields

    readonly DocumentThrownExceptionWarning _warning;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentThrownExceptionQuickFix"/> class.
    /// </summary>
    /// <param name="warning">The suggestion.</param>
    public DocumentThrownExceptionQuickFix(DocumentThrownExceptionWarning warning) {
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

        items.Add(new DocumentThrownExceptionBulbItem(_warning));

        return items.ToArray();
      }
    }

    #endregion
  }
}