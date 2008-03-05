namespace AgentJohnson.Exceptions {
  public partial class CatchExceptionsContextAction {
    /// <summary>
    /// Represents a Pair.
    /// </summary>
    public class Pair<TKey, TValue> {
      #region Fields

      TKey _key;
      TValue _value;

      #endregion

      #region Public properties

      /// <summary>
      /// Gets or sets the key.
      /// </summary>
      /// <value>The key.</value>
      public TKey Key {
        get {
          return _key;
        }
        set {
          _key = value;
        }
      }

      /// <summary>
      /// Gets or sets the value.
      /// </summary>
      /// <value>The value.</value>
      public TValue Value {
        get {
          return _value;
        }
        set {
          _value = value;
        }
      }

      #endregion
    }
  }
}