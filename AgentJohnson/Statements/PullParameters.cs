// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PullParameters.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the pull parameters class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Statements
{
  using System.Text;
  using JetBrains.Application;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Defines the pull parameters class.
  /// </summary>
  [ContextAction(Description = "Pulls the containing methods parameters to this method call.", Name = "Pull parameters", Priority = -1, Group = "C#")]
  public class PullParameters : ContextActionBase
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PullParameters"/> class.
    /// </summary>
    /// <param name="provider">
    /// The provider.
    /// </param>
    public PullParameters(ICSharpContextActionDataProvider provider) : base(provider)
    {
      this.StartTransaction = false;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes this instance.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    protected override void Execute(IElement element)
    {
      using (this.TextControl.Document.EnsureWritable())
      {
        try
        {
          CommandProcessor.Instance.BeginCommand("PullParameters");

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

          this.HandleEmptyParentheses(element);
        }
        finally
        {
          CommandProcessor.Instance.EndCommand();
        }
      }
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <returns>
    /// The text.
    /// </returns>
    protected override string GetText()
    {
      return "Pull parameters [Agent Johnson]";
    }

    /// <summary>
    /// Determines whether this instance is available.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// <c>true</c> if this instance is available; otherwise, <c>false</c>.
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

      return IsEmptyParentheses(element);
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// The text.
    /// </returns>
    private static string GetText(IElement element)
    {
      var typeMemberDeclaration = element.GetContainingElement<ITypeMemberDeclaration>(true);
      if (typeMemberDeclaration == null)
      {
        return null;
      }

      var parametersOwner = typeMemberDeclaration.DeclaredElement as IParametersOwner;
      if (parametersOwner == null)
      {
        return null;
      }

      if (parametersOwner.Parameters.Count == 0)
      {
        return null;
      }

      var first = true;
      var parametersBuilder = new StringBuilder();

      foreach (var parameter in parametersOwner.Parameters)
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

    /// <summary>
    /// Handles the empty parentheses.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// <c>true</c> if [is empty parentheses] [the specified element]; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsEmptyParentheses(IElement element)
    {
      var text = element.GetText();
      if (text != ")")
      {
        return false;
      }

      var invocationExpression = element.ToTreeNode().Parent as IInvocationExpression;
      if (invocationExpression == null)
      {
        return false;
      }

      var arguments = invocationExpression.Arguments;
      if (arguments.Count != 0)
      {
        return false;
      }

      var containingTypeMemberDeclaration = invocationExpression.GetContainingTypeMemberDeclaration();
      if (containingTypeMemberDeclaration == null)
      {
        return false;
      }

      var parametersOwner = containingTypeMemberDeclaration.DeclaredElement as IParametersOwner;
      if (parametersOwner == null)
      {
        return false;
      }

      if (parametersOwner.Parameters.Count == 0)
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Handles the end of line.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// <c>true</c> if [is expression statement] [the specified element]; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsExpressionStatement(IElement element)
    {
      var treeNode = element.ToTreeNode();

      if (!(treeNode.Parent is IChameleonNode))
      {
        return false;
      }

      var expressionStatement = treeNode.PrevSibling as IExpressionStatement;
      if (expressionStatement == null)
      {
        return false;
      }

      var text = expressionStatement.GetText();

      if (text.EndsWith(";"))
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Determines whether [is reference expression] [the specified element].
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// <c>true</c> if [is reference expression] [the specified element]; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsReferenceExpression(IElement element)
    {
      var treeNode = element.ToTreeNode();

      if (treeNode.Parent is IExpressionStatement && treeNode.PrevSibling is IReferenceExpression)
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// Handles the empty parentheses.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    private void HandleEmptyParentheses(IElement element)
    {
      var text = GetText(element);
      this.TextControl.Document.InsertText(this.TextControl.CaretModel.Offset, text);
    }

    /// <summary>
    /// Handles the end of line.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    private void HandleExpressionStatement(IElement element)
    {
      var text = GetText(element);
      this.TextControl.Document.InsertText(this.TextControl.CaretModel.Offset, "(" + text + ");");
    }

    /// <summary>
    /// Handles the reference expression.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    private void HandleReferenceExpression(IElement element)
    {
      var text = GetText(element);
      this.TextControl.Document.InsertText(this.TextControl.CaretModel.Offset, "(" + text + ")");
    }

    #endregion
  }
}