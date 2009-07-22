// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReturnQuickFix.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the return quick fix class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
    #region Constants and Fields

    /// <summary>
    /// The warning.
    /// </summary>
    private readonly ReturnWarning warning;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ReturnQuickFix"/> class.
    /// </summary>
    /// <param name="warning">
    /// The suggestion.
    /// </param>
    public ReturnQuickFix(ReturnWarning warning)
    {
      this.warning = warning;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>The items.</value>
    public IBulbItem[] Items
    {
      get
      {
        var items = new List<IBulbItem>
        {
          new ReturnBulbItem(this.warning)
        };

        return items.ToArray();
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IBulbAction

    /// <summary>
    /// Check if this action is available at the constructed context.
    /// Actions could store precalculated info in <paramref name="cache"/> to share it between different actions
    /// </summary>
    /// <param name="cache">
    /// </param>
    /// <returns>
    /// The is available.
    /// </returns>
    public bool IsAvailable(IUserDataHolder cache)
    {
      return true;
    }

    #endregion

    #endregion
  }
}