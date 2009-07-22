// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvertReturnValueRefactoring.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Represents a Refactoring.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Refactorings
{
  using Strings;
  using JetBrains.Application;
  using JetBrains.DocumentModel;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.TextControl;
  using JetBrains.Util;

  /// <summary>
  /// Represents a Refactoring.
  /// </summary>
  public class InvertReturnValueRefactoring
  {
    #region Constants and Fields

    /// <summary>
    /// The _solution.
    /// </summary>
    private readonly ISolution _solution;

    /// <summary>
    /// The _text control.
    /// </summary>
    private readonly ITextControl _textControl;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="InvertReturnValueRefactoring"/> class. 
    /// Initializes a new instance of the <see cref="IntroduceStringConstantRefactoring"/> class.
    /// </summary>
    /// <param name="solution">
    /// The solution.
    /// </param>
    /// <param name="textControl">
    /// The text control.
    /// </param>
    public InvertReturnValueRefactoring(ISolution solution, ITextControl textControl)
    {
      this._solution = solution;
      this._textControl = textControl;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the solution.
    /// </summary>
    /// <value>The solution.</value>
    public ISolution Solution
    {
      get
      {
        return this._solution;
      }
    }

    /// <summary>
    /// Gets the text control.
    /// </summary>
    /// <value>The text control.</value>
    public ITextControl TextControl
    {
      get
      {
        return this._textControl;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Determines whether the specified solution is available.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// <c>true</c> if the specified solution is available; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAvailable(IElement element)
    {
      Shell.Instance.Locks.AssertReadAccessAllowed();

      var typeMemberDeclaration = element.ToTreeNode().Parent as ITypeMemberDeclaration;
      if (typeMemberDeclaration == null)
      {
        return false;
      }

      var function = typeMemberDeclaration.DeclaredElement as IFunction;
      if (function == null)
      {
        return false;
      }

      var type = function.ReturnType;

      var name = type.GetPresentableName(element.Language);

      return name == "bool";
    }

    /// <summary>
    /// Executes this instance.
    /// </summary>
    public void Execute()
    {
      var element = this.GetElementAtCaret();
      if (element == null)
      {
        return;
      }

      var typeMemberDeclaration = element.ToTreeNode().Parent as ITypeMemberDeclaration;
      if (typeMemberDeclaration == null)
      {
        return;
      }

      using (var cookie = this.TextControl.Document.EnsureWritable())
      {
        if (cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS)
        {
          return;
        }

        using (CommandCookie.Create("Context Action Invert Return Value"))
        {
          PsiManager.GetInstance(this.Solution).DoTransaction(delegate { this.Execute(typeMemberDeclaration); });
        }
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Determines whether [is not expression] [the specified value].
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <returns>
    /// <c>true</c> if [is not expression] [the specified value]; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsNotExpression(ICSharpExpression value)
    {
      var unaryOperatorExpression = value as IUnaryOperatorExpression;
      if (unaryOperatorExpression == null)
      {
        return false;
      }

      var sign = unaryOperatorExpression.OperatorSign;
      if (sign == null)
      {
        return false;
      }

      var operatorSign = sign.GetText();

      return operatorSign == "!";
    }

    /// <summary>
    /// Executes the specified type member declaration.
    /// </summary>
    /// <param name="typeMemberDeclaration">
    /// The type member declaration.
    /// </param>
    private void Execute(ITypeMemberDeclaration typeMemberDeclaration)
    {
      var processor = new RecursiveElementProcessor(this.ReplaceReturnValue);

      typeMemberDeclaration.ProcessDescendants(processor);
    }

    /// <summary>
    /// Gets the element at caret.
    /// </summary>
    /// <returns>
    /// The element at caret.
    /// </returns>
    private IElement GetElementAtCaret()
    {
      var projectFile = DocumentManager.GetInstance(this.Solution).GetProjectFile(this.TextControl.Document);
      if (projectFile == null)
      {
        return null;
      }

      var file = PsiManager.GetInstance(this.Solution).GetPsiFile(projectFile) as ICSharpFile;
      if (file == null)
      {
        return null;
      }

      return file.FindTokenAt(this.TextControl.CaretModel.Offset);
    }

    /// <summary>
    /// Actions the specified t.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    private void ReplaceReturnValue(IElement element)
    {
      var returnStatement = element as IReturnStatement;
      if (returnStatement == null)
      {
        return;
      }

      var value = returnStatement.Value;
      if (value == null)
      {
        return;
      }

      var factory = CSharpElementFactory.GetInstance(element.GetPsiModule());
      if (factory == null)
      {
        return;
      }

      ICSharpExpression expression;

      var text = value.GetText();

      if (text == "true")
      {
        expression = factory.CreateExpression("false");
      }
      else if (text == "false")
      {
        expression = factory.CreateExpression("true");
      }
      else if (IsNotExpression(value))
      {
        var unaryOperatorExpression = (IUnaryOperatorExpression)value;

        text = unaryOperatorExpression.Operand.GetText();
        if (text.StartsWith("(") && text.EndsWith(")"))
        {
          text = text.Substring(1, text.Length - 2);
        }

        expression = factory.CreateExpression(text);
      }
      else
      {
        if (text.StartsWith("(") && text.EndsWith(")"))
        {
          text = text.Substring(1, text.Length - 2);
        }

        expression = factory.CreateExpression("!(" + text + ")");
      }

      if (expression == null)
      {
        return;
      }

      value.ReplaceBy(expression);
    }

    #endregion
  }
}