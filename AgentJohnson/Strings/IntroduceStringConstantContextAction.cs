namespace AgentJohnson.Strings
{
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Defines the introduce string constant context action class.
  /// </summary>
  [ContextAction(Description = "Generates a string constant from the literal string.", Name = "Introduce string constant", Priority = -1, Group = "C#")]
  public class IntroduceStringConstantContextAction : ContextActionBase
  {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="IntroduceStringConstantContextAction"/> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    public IntroduceStringConstantContextAction(ICSharpContextActionDataProvider provider) : base(provider)
    {
      this.StartTransaction = false;
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes this instance.
    /// </summary>
    /// <param name="element">The element.</param>
    protected override void Execute(IElement element)
    {
      IntroduceStringConstantRefactoring introduceStringConstantRefactoring = new IntroduceStringConstantRefactoring(this.Solution, this.TextControl);

      introduceStringConstantRefactoring.Execute();
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <returns>The text.</returns>
    protected override string GetText()
    {
      return "Introduce String Constant";
    }

    /// <summary>
    /// Determines whether this instance is available.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>
    /// 	<c>true</c> if this instance is available; otherwise, <c>false</c>.
    /// </returns>
    protected override bool IsAvailable(IElement element)
    {
      return IntroduceStringConstantRefactoring.IsAvailable(element);
    }

    #endregion
  }
}