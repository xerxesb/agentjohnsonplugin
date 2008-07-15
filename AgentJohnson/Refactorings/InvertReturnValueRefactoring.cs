using AgentJohnson.Strings;
using JetBrains.Application;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace AgentJohnson.Refactorings {
  /// <summary>
  /// Represents a Refactoring.
  /// </summary>
  public class InvertReturnValueRefactoring {
    #region Fields

    readonly ISolution _solution;
    readonly ITextControl _textControl;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="IntroduceStringConstantRefactoring"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public InvertReturnValueRefactoring(ISolution solution, ITextControl textControl) {
      _solution = solution;
      _textControl = textControl;
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the solution.
    /// </summary>
    /// <value>The solution.</value>
    public ISolution Solution {
      get {
        return _solution;
      }
    }

    /// <summary>
    /// Gets the text control.
    /// </summary>
    /// <value>The text control.</value>
    public ITextControl TextControl {
      get {
        return _textControl;
      }
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Executes this instance.
    /// </summary>
    public void Execute() {
      IElement element = GetElementAtCaret();
      if(element == null) {
        return;
      }

      ITypeMemberDeclaration typeMemberDeclaration = element.ToTreeNode().Parent as ITypeMemberDeclaration;
      if(typeMemberDeclaration == null) {
        return;
      }

      using(ModificationCookie cookie = TextControl.Document.EnsureWritable()) {
        if(cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS) {
          return;
        }

        using(CommandCookie.Create("Context Action Invert Return Value")) {
          PsiManager.GetInstance(Solution).DoTransaction(delegate { Execute(typeMemberDeclaration); });
        }
      }
    }

    /// <summary>
    /// Executes the specified type member declaration.
    /// </summary>
    /// <param name="typeMemberDeclaration">The type member declaration.</param>
    void Execute(ITypeMemberDeclaration typeMemberDeclaration) {
      RecursiveElementProcessor processor = new RecursiveElementProcessor(ReplaceReturnValue);

      typeMemberDeclaration.ProcessDescendants(processor);
    }

    /// <summary>
    /// Actions the specified t.
    /// </summary>
    /// <param name="element">The element.</param>
    void ReplaceReturnValue(IElement element) {
      IReturnStatement returnStatement = element as IReturnStatement;
      if(returnStatement == null) {
        return;
      }

      ICSharpExpression value = returnStatement.Value;
      if(value == null) {
        return;
      }

      CSharpElementFactory factory = CSharpElementFactory.GetInstance(element.GetProject());
      if(factory == null) {
        return;
      }

      ICSharpExpression expression;

      string text = value.GetText();

      if(text == "true") {
        expression = factory.CreateExpression("false");
      }
      else if(text == "false") {
        expression = factory.CreateExpression("true");
      }
      else if(IsNotExpression(value)) {
        IUnaryOperatorExpression unaryOperatorExpression = (IUnaryOperatorExpression)value;

        text = unaryOperatorExpression.Operand.GetText();
        if(text.StartsWith("(") && text.EndsWith(")")) {
          text = text.Substring(1, text.Length - 2);
        }

        expression = factory.CreateExpression(text);
      }
      else {
        if(text.StartsWith("(") && text.EndsWith(")")) {
          text = text.Substring(1, text.Length - 2);
        }

        expression = factory.CreateExpression("!(" + text + ")");
      }

      if(expression == null) {
        return;
      }

      value.ReplaceBy(expression);
    }

    /// <summary>
    /// Determines whether [is not expression] [the specified value].
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>
    /// 	<c>true</c> if [is not expression] [the specified value]; otherwise, <c>false</c>.
    /// </returns>
    static bool IsNotExpression(ICSharpExpression value) {
      IUnaryOperatorExpression unaryOperatorExpression = value as IUnaryOperatorExpression;
      if(unaryOperatorExpression == null) {
        return false;
      }

      ITokenNode sign = unaryOperatorExpression.OperatorSign;
      if(sign == null) {
        return false;
      }

      string operatorSign = sign.GetText();

      return operatorSign == "!";
    }

    /// <summary>
    /// Determines whether the specified solution is available.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>
    /// 	<c>true</c> if the specified solution is available; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsAvailable(IElement element) {
      Shell.Instance.Locks.AssertReadAccessAllowed();

      ITypeMemberDeclaration typeMemberDeclaration = element.ToTreeNode().Parent as ITypeMemberDeclaration;
      if(typeMemberDeclaration == null) {
        return false;
      }

      IFunction function = typeMemberDeclaration as IFunction;
      if(function == null) {
        return false;
      }

      IType type = function.ReturnType;

      string name = type.GetPresentableName(element.Language);
      
      return name == "bool";
    }

    /// <summary>
    /// Gets the element at caret.
    /// </summary>
    /// <returns>The element at caret.</returns>
    IElement GetElementAtCaret() {
      IProjectFile projectFile = DocumentManager.GetInstance(Solution).GetProjectFile(TextControl.Document);
      if(projectFile == null) {
        return null;
      }

      ICSharpFile file = PsiManager.GetInstance(Solution).GetPsiFile(projectFile) as ICSharpFile;
      if(file == null) {
        return null;
      }

      return file.FindTokenAt(TextControl.CaretModel.Offset);
    }

    #endregion
  }
}