// <copyright file="NegateIfCondition.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>

namespace AgentJohnson.Statements
{
  using JetBrains.Application;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Daemon;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.TextControl;
  using JetBrains.Util;

  /// <summary>
  /// Defines the negate if condition class.
  /// </summary>
  [ContextAction(Description = "Negates the condition of an 'if' statement.", Name = "Negate 'if' condition", Priority = -1, Group = "C#")]
  public class NegateIfCondition : ContextActionBase
  {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="NegateIfCondition"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public NegateIfCondition(ISolution solution, ITextControl textControl) : base(solution, textControl)
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
      using (ModificationCookie cookie = this.TextControl.Document.EnsureWritable())
      {
        if (cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS)
        {
          return;
        }

        using (CommandCookie.Create("Context Action Negate If Condition"))
        {
          PsiManager.GetInstance(this.Solution).DoTransaction(delegate { Negate(element); });
        }
      }
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <returns>The text in the context menu.</returns>
    protected override string GetText()
    {
      return "Negate 'if' condition";
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
      IIfStatement ifStatement = element.GetContainingElement<IIfStatement>(true);
      if (ifStatement == null)
      {
        return false;
      }

      IExpression condition = ifStatement.Condition;
      if (condition == null)
      {
        return false;
      }

      if (!condition.Contains(element))
      {
        return false;
      }

      return true;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Reverses the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    private static void Negate(IElement element)
    {
      IIfStatement ifStatement = element.GetContainingElement<IIfStatement>(true);
      if (ifStatement == null)
      {
        return;
      }

      IExpression condition = ifStatement.Condition;
      if (condition == null)
      {
        return;
      }

      CSharpElementFactory factory = CSharpElementFactory.GetInstance(element.GetProject());
      if (factory == null)
      {
        return;
      }

      IEqualityExpression equalityExpression = ifStatement.Condition as IEqualityExpression;
      if (equalityExpression != null)
      {
        NegateEqualityExpression(factory, equalityExpression);
        return;
      }

      IRelationalExpression relationalExpression = ifStatement.Condition as IRelationalExpression;
      if (relationalExpression != null)
      {
        NegateRelationalExpression(factory, relationalExpression);
        return;
      }

      IUnaryOperatorExpression unaryOperatorExpression = ifStatement.Condition as IUnaryOperatorExpression;
      if (unaryOperatorExpression != null)
      {
        NegateUnaryExpression(factory, unaryOperatorExpression);
        return;
      }

      IInvocationExpression invocationExpression = ifStatement.Condition as IInvocationExpression;
      if (invocationExpression != null)
      {
        NegateInvocationExpression(factory, invocationExpression);
        return;
      }

      ILiteralExpression literalExpression = ifStatement.Condition as ILiteralExpression;
      if (literalExpression != null)
      {
        NegateLiteralExpression(factory, literalExpression);
        return;
      }

      NegateExpression(factory, ifStatement.Condition);
    }

    /// <summary>
    /// Negates the equality expression.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="equalityExpression">The equality expression.</param>
    private static void NegateEqualityExpression(CSharpElementFactory factory, IEqualityExpression equalityExpression)
    {
      string operatorSign = equalityExpression.OperatorSign.GetText();

      operatorSign = (operatorSign == "==" ? "!=" : "==");

      ICSharpExpression expression = factory.CreateExpression(string.Format("{0} {1} {2}", equalityExpression.LeftOperand.GetText(), operatorSign, equalityExpression.RightOperand.GetText()));

      equalityExpression.ReplaceBy(expression);
    }

    /// <summary>
    /// Negates the expression.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="condition">The condition.</param>
    private static void NegateExpression(CSharpElementFactory factory, ICSharpExpression condition)
    {
      ICSharpExpression expression = factory.CreateExpression("!(" + condition.GetText() + ")");

      condition.ReplaceBy(expression);
    }

    /// <summary>
    /// Negates the invocation expression.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="invocationExpression">The invocation expression.</param>
    private static void NegateInvocationExpression(CSharpElementFactory factory, IInvocationExpression invocationExpression)
    {
      ICSharpExpression expression = factory.CreateExpression("!" + invocationExpression.GetText());

      invocationExpression.ReplaceBy(expression);
    }

    /// <summary>
    /// Negates the literal expression.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="literalExpression">The literal expression.</param>
    private static void NegateLiteralExpression(CSharpElementFactory factory, ILiteralExpression literalExpression)
    {
      string text = literalExpression.GetText();

      if (text == "true")
      {
        text = "false";
      }
      else if (text == "false")
      {
        text = "true";
      }
      else
      {
        return;
      }

      ICSharpExpression expression = factory.CreateExpression(text);

      literalExpression.ReplaceBy(expression);
    }

    /// <summary>
    /// Negates the relational expression.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="relationalExpression">The relational expression.</param>
    private static void NegateRelationalExpression(CSharpElementFactory factory, IRelationalExpression relationalExpression)
    {
      string operatorSign = relationalExpression.OperatorSign.GetText();

      switch (operatorSign)
      {
        case "<":
          operatorSign = ">=";
          break;
        case ">":
          operatorSign = "<=";
          break;
        case "<=":
          operatorSign = ">";
          break;
        case ">=":
          operatorSign = "<";
          break;
      }

      ICSharpExpression expression = factory.CreateExpression(string.Format("{0} {1} {2}", relationalExpression.LeftOperand.GetText(), operatorSign, relationalExpression.RightOperand.GetText()));

      relationalExpression.ReplaceBy(expression);
    }

    /// <summary>
    /// Negates the unary expression.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="unaryOperatorExpression">The unary operator expression.</param>
    private static void NegateUnaryExpression(CSharpElementFactory factory, IUnaryOperatorExpression unaryOperatorExpression)
    {
      if (unaryOperatorExpression.OperatorSign.GetText() != "!")
      {
        return;
      }

      string text = unaryOperatorExpression.Operand.GetText().Trim();

      if (text.StartsWith("(") && text.EndsWith(")"))
      {
        text = text.Substring(1, text.Length - 2);
      }

      ICSharpExpression expression = factory.CreateExpression(text);

      unaryOperatorExpression.ReplaceBy(expression);
    }

    #endregion
  }
}