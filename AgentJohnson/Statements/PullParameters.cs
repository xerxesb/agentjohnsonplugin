namespace AgentJohnson.Statements
{
  using System.Text;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Defines the pull parameters class.
  /// </summary>
  [ContextAction(Description = "Pulls the containing methods parameters to this method call.", Name = "Pull parameters", Priority = 1, Group = "C#")]
  public class PullParameters : ContextActionBase
  {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="PullParameters"/> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    public PullParameters(ICSharpContextActionDataProvider provider) : base(provider)
    {
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes this instance.
    /// </summary>
    /// <param name="element">The element.</param>
    protected override void Execute(IElement element)
    {
      if (IsExpressionStatement(element))
      {
        this.HandleExpressionStatement(element);
        return;
      }

      if (IsReferenceExpression(element))
      {
        this.HandleReferenceExpression(element);
        return;
      }

      // HandleEmptyParentheses(element);
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <returns>The text.</returns>
    protected override string GetText()
    {
      return "Pull parameters";
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
      if (IsExpressionStatement(element))
      {
        return true;
      }

      if (IsReferenceExpression(element))
      {
        return true;
      }

      return false;

      // return IsEmptyParentheses(element);
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    private static string GetText(IElement element)
    {
      ITypeMemberDeclaration typeMemberDeclaration = element.GetContainingElement<ITypeMemberDeclaration>(true);
      if (typeMemberDeclaration == null)
      {
        return null;
      }

      IParametersOwner parametersOwner = typeMemberDeclaration.DeclaredElement as IParametersOwner;
      if (parametersOwner == null)
      {
        return null;
      }

      if (parametersOwner.Parameters.Count == 0)
      {
        return null;
      }

      bool first = true;
      StringBuilder parametersBuilder = new StringBuilder();

      foreach (IParameter parameter in parametersOwner.Parameters)
      {
        if (!first)
        {
          parametersBuilder.Append(", ");
        }
        first = false;

        parametersBuilder.Append(parameter.ShortName);
      }

      return parametersBuilder.ToString();
    }

    /*
    /// <summary>
    /// Handles the empty parentheses.
    /// </summary>
    /// <param name="element">The element.</param>
    void HandleEmptyParentheses(IElement element) {
      string text = GetText(element);
      TextControl.Document.InsertText(TextControl.CaretModel.Offset, text);
    }
    */

    /*
    /// <summary>
    /// Handles the empty parentheses.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns></returns>
   
    static bool IsEmptyParentheses(IElement element) {
      string text = element.GetText();
      if(text != ")") {
        return false;
      }

      IInvocationExpression invocationExpression = element.ToTreeNode().Parent as IInvocationExpression;
      if(invocationExpression == null) {
        return false;
      }

      IList<ICSharpArgument> arguments = invocationExpression.Arguments;
      if(arguments.Count != 0) {
        return false;
      }

      IParametersOwner parametersOwner = invocationExpression.GetContainingTypeMemberDeclaration() as IParametersOwner;
      if(parametersOwner == null) {
        return false;
      }

      if(parametersOwner.Parameters.Count == 0) {
        return false;
      }

      return true;
    }
    */

    /// <summary>
    /// Handles the end of line.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    private static bool IsExpressionStatement(IElement element)
    {
      ITreeNode treeNode = element.ToTreeNode();

      if (!(treeNode.Parent is IChameleonNode))
      {
        return false;
      }

      IExpressionStatement expressionStatement = treeNode.PrevSibling as IExpressionStatement;
      if (expressionStatement == null)
      {
        return false;
      }

      string text = expressionStatement.GetText();

      if (text.EndsWith(";"))
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Determines whether [is reference expression] [the specified element].
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>
    /// 	<c>true</c> if [is reference expression] [the specified element]; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsReferenceExpression(IElement element)
    {
      ITreeNode treeNode = element.ToTreeNode();

      if (treeNode.Parent is IExpressionStatement && treeNode.PrevSibling is IReferenceExpression)
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// Handles the end of line.
    /// </summary>
    /// <param name="element">The element.</param>
    private void HandleExpressionStatement(IElement element)
    {
      string text = GetText(element);
      this.TextControl.Document.InsertText(this.TextControl.CaretModel.Offset, "(" + text + ");");
    }

    /// <summary>
    /// Handles the reference expression.
    /// </summary>
    /// <param name="element">The element.</param>
    private void HandleReferenceExpression(IElement element)
    {
      string text = GetText(element);
      this.TextControl.Document.InsertText(this.TextControl.CaretModel.Offset, "(" + text + ")");
    }

    #endregion
  }
}