// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReverseForDirectionContextAction.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The reverse for context action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Statements
{
  using System.Text.RegularExpressions;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.DataProviders;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// The reverse for context action.
  /// </summary>
  [ContextAction(Description = "Reverses the direction of a for-loop.", Name = "Reverse for-loop direction", Priority = -1, Group = "C#")]
  public class ReverseForContextAction : ContextActionBase
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ReverseForContextAction"/> class.
    /// </summary>
    /// <param name="provider">
    /// The provider.
    /// </param>
    public ReverseForContextAction(ICSharpContextActionDataProvider provider) : base(provider)
    {
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
      var factory = CSharpElementFactory.GetInstance(element.GetPsiModule());
      if (factory == null)
      {
        return;
      }

      var statement = element.ToTreeNode().Parent as IForStatement;
      if (statement == null)
      {
        return;
      }

      var localVariableDeclarations = statement.InitializerDeclarations;
      if (localVariableDeclarations.Count != 1)
      {
        return;
      }

      var localVariableDeclaration = localVariableDeclarations[0];
      if (localVariableDeclaration == null)
      {
        return;
      }

      var expressionInitializer = localVariableDeclaration.Initial as IExpressionInitializer;
      if (expressionInitializer == null)
      {
        return;
      }

      var from = expressionInitializer.Value;

      var relationalExpression = statement.Condition as IRelationalExpression;
      if (relationalExpression == null)
      {
        return;
      }

      var to = relationalExpression.RightOperand;

      var iterators = statement.IteratorExpressions;
      if (iterators == null)
      {
        return;
      }

      if (iterators.Count != 1)
      {
        return;
      }

      var postfixOperatorExpression = iterators[0] as IPostfixOperatorExpression;
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
        var expression = AddToExpression(factory, to, '-');

        expressionInitializer.SetValue(expression);

        var condition = GetCondition(factory, relationalExpression.LeftOperand, relationalExpression.OperatorSign.GetText(), from);

        relationalExpression.ReplaceBy(condition);

        var dec = factory.CreateExpression(string.Format("{0}--", postfixOperatorExpression.Operand.GetText()));

        postfixOperatorExpression.ReplaceBy(dec);
      }
      else
      {
        var expression = AddToExpression(factory, from, '+');

        expressionInitializer.SetValue(to);

        var condition = GetCondition(factory, relationalExpression.LeftOperand, relationalExpression.OperatorSign.GetText(), expression);

        relationalExpression.ReplaceBy(condition);

        var inc = factory.CreateExpression(string.Format("{0}++", postfixOperatorExpression.Operand.GetText()));

        postfixOperatorExpression.ReplaceBy(inc);
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
      return "Reverse for-loop direction [Agent Johnson]";
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
      var statement = element.ToTreeNode().Parent as IForStatement;
      if (statement == null)
      {
        return false;
      }

      var localVariableDeclarations = statement.InitializerDeclarations;
      if (localVariableDeclarations.Count != 1)
      {
        return false;
      }

      var localVariableDeclaration = localVariableDeclarations[0];
      if (localVariableDeclaration == null)
      {
        return false;
      }

      var localVariable = localVariableDeclaration.DeclaredElement as ILocalVariable;
      if (localVariable == null)
      {
        return false;
      }

      if (localVariable.Type.GetPresentableName(element.Language) != "int")
      {
        return false;
      }

      var iterators = statement.IteratorExpressions;
      if (iterators == null)
      {
        return false;
      }

      if (iterators.Count != 1)
      {
        return false;
      }

      var postfixOperatorExpression = iterators[0] as IPostfixOperatorExpression;
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

    /// <summary>
    /// Adds to expression.
    /// </summary>
    /// <param name="factory">
    /// The factory.
    /// </param>
    /// <param name="expression">
    /// To.
    /// </param>
    /// <param name="sign">
    /// The sign.
    /// </param>
    /// <returns>
    /// </returns>
    private static ICSharpExpression AddToExpression(CSharpElementFactory factory, ICSharpExpression expression, char sign)
    {
      var sign2 = sign == '-' ? '+' : '-';

      var text = expression.GetText();

      if (text.StartsWith("(") && text.EndsWith(")"))
      {
        text = text.Substring(1, text.Length - 2);
      }

      var match = Regex.Match(text, "\\" + sign2 + "\\s*1\\s*$");
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

      var result = factory.CreateExpression(text);

      if (result.IsConstantValue())
      {
        result = factory.CreateExpressionByValue(result.ConstantValue);
      }

      return result;
    }

    /// <summary>
    /// Gets the condition.
    /// </summary>
    /// <param name="factory">
    /// The factory.
    /// </param>
    /// <param name="leftOperand">
    /// The left operand.
    /// </param>
    /// <param name="operatorSign">
    /// The operator sign.
    /// </param>
    /// <param name="rightOperand">
    /// The right operand.
    /// </param>
    /// <returns>
    /// </returns>
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