// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatchExceptionsContextActions.Pair.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The catch exceptions context action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Exceptions
{
  /// <summary>
  /// The catch exceptions context action.
  /// </summary>
  public partial class CatchExceptionsContextAction
  {
    /// <summary>
    /// Represents a Pair.
    /// </summary>
    /// <typeparam name="TKey">
    /// </typeparam>
    /// <typeparam name="TValue">
    /// </typeparam>
    public class Pair<TKey, TValue>
    {
      #region Constants and Fields

      /// <summary>
      /// The _key.
      /// </summary>
      private TKey _key;

      /// <summary>
      /// The _value.
      /// </summary>
      private TValue _value;

      #endregion

      #region Properties

      /// <summary>
      /// Gets or sets the key.
      /// </summary>
      /// <value>The key.</value>
      public TKey Key
      {
        get
        {
          return this._key;
        }

        set
        {
          this._key = value;
        }
      }

      /// <summary>
      /// Gets or sets the value.
      /// </summary>
      /// <value>The value.</value>
      public TValue Value
      {
        get
        {
          return this._value;
        }

        set
        {
          this._value = value;
        }
      }

      #endregion
    }
  }
}