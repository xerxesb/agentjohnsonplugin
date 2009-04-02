namespace AgentJohnson.Statements
{
  using System.Collections.Generic;
  using System.Text.RegularExpressions;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// 
  /// </summary>
  [ContextAction(Description = "Reverses the direction of a for-loop.", Name = "Reverse for-loop direction", Priority = -1, Group = "C#")]
  public class ReverseForContextAction : ContextActionBase
  {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ReverseForContextAction"/> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    public ReverseForContextAction(ICSharpContextActionDataProvider provider) : base(provider)
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
      CSharpElementFactory factory = CSharpElementFactory.GetInstance(element.GetPsiModule());
      if (factory == null)
      {
        return;
      }

      IForStatement statement = element.ToTreeNode().Parent as IForStatement;
      if (statement == null)
      {
        return;
      }

      IList<ILocalVariableDeclaration> localVariableDeclarations = statement.InitializerDeclarations;
      if (localVariableDeclarations.Count != 1)
      {
        return;
      }

      ILocalVariableDeclaration localVariableDeclaration = localVariableDeclarations[0];
      if (localVariableDeclaration == null)
      {
        return;
      }

      IExpressionInitializer expressionInitializer = localVariableDeclaration.Initial as IExpressionInitializer;
      if (expressionInitializer == null)
      {
        return;
      }

      ICSharpExpression from = expressionInitializer.Value;

      IRelationalExpression relationalExpression = statement.Condition as IRelationalExpression;
      if (relationalExpression == null)
      {
        return;
      }

      ICSharpExpression to = relationalExpression.RightOperand;

      IList<ICSharpExpression> iterators = statement.IteratorExpressions;
      if (iterators == null)
      {
        return;
      }

      if (iterators.Count != 1)
      {
        return;
      }

      IPostfixOperatorExpression postfixOperatorExpression = iterators[0] as IPostfixOperatorExpression;
      if (postfixOperatorExpression == null)
      {
        return;
      }

      if (postfixOperatorExpression.PostfixOperatorType == PostfixOperatorType.INVALID)
      {
        return;
      }

      if (postfixOperatorExpression.PostfixOperatorType == PostfixOperatorType.PLUSPLUS)
      {
        ICSharpExpression expression = AddToExpression(factory, to, '-');

        expressionInitializer.SetValue(expression);

        ICSharpExpression condition = GetCondition(factory, relationalExpression.LeftOperand, relationalExpression.OperatorSign.GetText(), from);

        relationalExpression.ReplaceBy(condition);

        ICSharpExpression dec = factory.CreateExpression(string.Format("{0}--", postfixOperatorExpression.Operand.GetText()));

        postfixOperatorExpression.ReplaceBy(dec);
      }
      else
      {
        ICSharpExpression expression = AddToExpression(factory, from, '+');

        expressionInitializer.SetValue(to);

        ICSharpExpression condition = GetCondition(factory, relationalExpression.LeftOperand, relationalExpression.OperatorSign.GetText(), expression);

        relationalExpression.ReplaceBy(condition);

        ICSharpExpression inc = factory.CreateExpression(string.Format("{0}++", postfixOperatorExpression.Operand.GetText()));

        postfixOperatorExpression.ReplaceBy(inc);
      }
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <returns>The text.</returns>
    protected override string GetText()
    {
      return "Reverse for-loop direction [Agent Johnson]";
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
      IForStatement statement = element.ToTreeNode().Parent as IForStatement;
      if (statement == null)
      {
        return false;
      }

      IList<ILocalVariableDeclaration> localVariableDeclarations = statement.InitializerDeclarations;
      if (localVariableDeclarations.Count != 1)
      {
        return false;
      }

      ILocalVariableDeclaration localVariableDeclaration = localVariableDeclarations[0];
      if (localVariableDeclaration == null)
      {
        return false;
      }

      ILocalVariable localVariable = localVariableDeclaration as ILocalVariable;
      if (localVariable == null)
      {
        return false;
      }

      if (localVariable.Type.GetPresentableName(element.Language) != "int")
      {
        return false;
      }

      IList<ICSharpExpression> iterators = statement.IteratorExpressions;
      if (iterators == null)
      {
        return false;
      }

      if (iterators.Count != 1)
      {
        return false;
      }

      IPostfixOperatorExpression postfixOperatorExpression = iterators[0] as IPostfixOperatorExpression;
      if (postfixOperatorExpression == null)
      {
        return false;
      }

      if (postfixOperatorExpression.PostfixOperatorType == PostfixOperatorType.INVALID)
      {
        return false;
      }

      return true;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Adds to expression.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="expression">To.</param>
    /// <param name="sign">The sign.</param>
    /// <returns></returns>
    private static ICSharpExpression AddToExpression(CSharpElementFactory factory, ICSharpExpression expression, char sign)
    {
      char sign2 = sign == '-' ? '+' : '-';

      string text = expression.GetText();

      if (text.StartsWith("(") && text.EndsWith(")"))
      {
        text = text.Substring(1, text.Length - 2);
      }

      Match match = Regex.Match(text, "\\" + sign2 + "\\s*1\\s*$");
      if (match.Success)
      {
        text = text.Substring(0, text.Length - match.Value.Length).Trim();

        if (text.StartsWith("(") && text.EndsWith(")"))
        {
          text = text.Substring(1, text.Length - 2);
        }
      }
      else
      {
        if (expression is IBinaryExpression)
        {
          text = "(" + text + ") " + sign + " 1";
        }
        else
        {
          text += sign + " 1";
        }
      }

      text = text.Trim();

      ICSharpExpression result = factory.CreateExpression(text);

      if (result.IsConstantValue())
      {
        result = factory.CreateExpressionByValue(result.ConstantValue);
      }

      return result;
    }

    /// <summary>
    /// Gets the condition.
    /// </summary>
    /// <param name="factory">The factory.</param>
    /// <param name="leftOperand">The left operand.</param>
    /// <param name="operatorSign">The operator sign.</param>
    /// <param name="rightOperand">The right operand.</param>
    /// <returns></returns>
    private static ICSharpExpression GetCondition(CSharpElementFactory factory, ICSharpExpression leftOperand, string operatorSign, ICSharpExpression rightOperand)
    {
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

      return factory.CreateExpression(string.Format("{0} {1} {2}", leftOperand.GetText(), operatorSign, rightOperand.GetText()));
    }

    #endregion
  }
}