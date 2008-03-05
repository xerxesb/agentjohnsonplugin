using AgentJohnson;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TextControl;

namespace AgentJohnson.Strings {
  /// <summary>
  /// </summary>
  [ContextAction(Description="Generates a string constant from the literal string.", Name="Introduce string constant", Priority=-1, Group="C#")]
  public class IntroduceStringConstantContextAction : ContextActionBase {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="IntroduceStringConstantContextAction"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public IntroduceStringConstantContextAction(ISolution solution, ITextControl textControl) : base(solution, textControl) {
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes this instance.
    /// </summary>
    /// <param name="element">The element.</param>
    protected override void Execute(IElement element) {
      Refactoring refactoring = new Refactoring(Solution, TextControl);

      refactoring.Execute();
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <returns>The text.</returns>
    protected override string GetText() {
      return "Introduce String Constant";
    }

    /// <summary>
    /// Determines whether this instance is available.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>
    /// 	<c>true</c> if this instance is available; otherwise, <c>false</c>.
    /// </returns>
    protected override bool IsAvailable(IElement element) {
      return Refactoring.IsAvailable(element);
    }

    #endregion
  }
}