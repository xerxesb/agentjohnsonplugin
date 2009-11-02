// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentThrownExceptionQuickFix.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the document thrown exception quick fix class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Exceptions
{
  using System.Collections.Generic;
  using JetBrains.ReSharper.Feature.Services.Bulbs;
  using JetBrains.ReSharper.Intentions;

  /// <summary>
  /// Defines the document thrown exception quick fix class.
  /// </summary>
  [QuickFix]
  public class DocumentThrownExceptionQuickFix : IQuickFix
  {
    #region Constants and Fields

    /// <summary>
    /// The _warning.
    /// </summary>
    private readonly DocumentThrownExceptionWarning warning;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentThrownExceptionQuickFix"/> class.
    /// </summary>
    /// <param name="warning">
    /// The suggestion.
    /// </param>
    public DocumentThrownExceptionQuickFix(DocumentThrownExceptionWarning warning)
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
          new DocumentThrownExceptionBulbItem(this.warning)
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
    public bool IsAvailable(global::JetBrains.Util.IUserDataHolder cache)
    {
      return true;
    }

    #endregion

    #endregion
  }
}