namespace AgentJohnson.ValueAnalysis
{
  using System.Collections.Generic;
  using JetBrains.ReSharper.Feature.Services.Bulbs;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.Util;

  /// <summary>
  /// Defines the return quick fix class.
  /// </summary>
  [QuickFix]
  public class ReturnQuickFix : IQuickFix
  {
    #region Fields

    private readonly ReturnWarning warning;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ReturnQuickFix"/> class.
    /// </summary>
    /// <param name="warning">The suggestion.</param>
    public ReturnQuickFix(ReturnWarning warning)
    {
      this.warning = warning;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Check if this action is available at the constructed context.
    /// Actions could store precalculated info in <paramref name="cache"/> to share it between different actions
    /// </summary>
    /// <param name="cache"></param>
    /// <returns></returns>
    public bool IsAvailable(IUserDataHolder cache)
    {
      return true;
    }

    #endregion

    #region IQuickFix Members

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>The items.</value>
    public IBulbItem[] Items
    {
      get
      {
        List<IBulbItem> items = new List<IBulbItem>
        {
          new ReturnBulbItem(this.warning)
        };

        return items.ToArray();
      }
    }

    #endregion
  }
}