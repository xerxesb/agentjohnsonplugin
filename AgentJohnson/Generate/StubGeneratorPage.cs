using JetBrains.ReSharper.CodeInsight.Services.Generate;
using JetBrains.UI.RichText;

namespace AgentJohnson.Generate {
  /// <summary>
  /// </summary>
  public class StubGeneratorPage : IGeneratePage {
    // int _modifierSelection;

    #region Private methods

    /// <summary>
    /// Determines whether this instance can show the specified processor.
    /// </summary>
    /// <param name="processor">The processor.</param>
    /// <returns>
    /// 	<c>true</c> if this instance can show the specified processor; otherwise, <c>false</c>.
    /// </returns>
    bool IGeneratePage.CanShow(GenerateProcessor processor) {
      return false;
      // return true;
    }

    /*
    /// <summary>
    /// Gets the modifiers.
    /// </summary>
    /// <value>The modifiers.</value>
    [Radio("&Modifiers:")]
    public string[] Modifiers {
      get {
         return new[] { "Public", "Protected", "Protected Internal", "Internal", "Private" };
      }
    }


    /// <summary>
    /// Gets or sets the modifiers selection.
    /// </summary>
    /// <value>The modifiers selection.</value>
    [RadioSelectionAttribute("&Modifiers:")]
    public int ModifiersSelection {
      get {
        return _modifierSelection; 
      }
      set {
        _modifierSelection = value;
      }
    }
    */

    #endregion

    #region IGeneratePage Members

    /// <summary>
    /// Gets a value indicating whether this instance can continue.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance can continue; otherwise, <c>false</c>.
    /// </value>
    bool IGeneratePage.CanContinue {
      get {
        return true;
      }
    }

    /// <summary>
    /// Gets the title.
    /// </summary>
    /// <value>The title.</value>
    RichText IGeneratePage.Title {
      get {
        return "Generate Method Stub";
      }
    }

    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <value>The description.</value>
    RichText IGeneratePage.Description {
      get {
        return "Generate Method Stub";
      }
    }

    #endregion
  }
}