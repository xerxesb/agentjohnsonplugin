using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Intentions.CSharp.ContextActions.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace AgentJohnson.Strings {
  /// <summary>
  /// Represents the Context Action.
  /// </summary>
  [ContextAction(Description = "Adds an 'if' statement after the current statement that checks if the string variable is null or empty.", Name = "Check if string is null or empty", Priority = -1, Group = "C#")]
  public class CheckStringNotNullOrEmptyContextAction : OneItemContextActionBase {
    #region Fields

    string _name;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckStringNotNullOrEmptyContextAction"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public CheckStringNotNullOrEmptyContextAction(ISolution solution, ITextControl textControl) : base(solution, textControl) {
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    public override string Text {
      get {
        return string.Format("Check if '{0}' is null or empty", _name ?? "[unknown]");
      }
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes the internal.
    /// </summary>
    protected override void ExecuteInternal() {
      if(!IsAvailableInternal()) {
        return;
      }

      IAssignmentExpression assignmentExpression = Provider.GetSelectedElement<IAssignmentExpression>(true, true);
      if(assignmentExpression != null) {
        CheckStringAssignment(assignmentExpression);
        return;
      }

      ILocalVariableDeclaration localVariableDeclaration = Provider.GetSelectedElement<ILocalVariableDeclaration>(true, true);
      if(localVariableDeclaration != null) {
        CheckStringAssignment(localVariableDeclaration);
        return;
      }
    }

    /// <summary>
    /// Called to check if ContextAction is available.
    /// ReadLock is taken
    /// Will not be called if <c>PsiManager</c>, ProjectFile of Solution == null
    /// </summary>
    /// <returns></returns>
    protected override bool IsAvailableInternal() {
      _name = null;

      ILocalVariableDeclaration localVariableDeclaration = Provider.GetSelectedElement<ILocalVariableDeclaration>(true, true);
      IAssignmentExpression assignmentExpression = Provider.GetSelectedElement<IAssignmentExpression>(true, true);

      if(assignmentExpression == null && localVariableDeclaration == null) {
        return false;
      }

      TextRange range;

      if(assignmentExpression != null) {
        ICSharpExpression destination = assignmentExpression.Dest;
        if(destination == null) {
          return false;
        }

        IType declaredType = destination.GetExpressionType() as IDeclaredType;
        if(declaredType == null || declaredType.GetPresentableName(destination.Language) != "string") {
          return false;
        }

        if(!destination.IsClassifiedAsVariable) {
          return false;
        }

        IReferenceExpression referenceExpression = assignmentExpression.Dest as IReferenceExpression;
        if(referenceExpression == null) {
          return false;
        }

        _name = referenceExpression.Reference.GetName();

        range = new TextRange(assignmentExpression.Dest.GetTreeStartOffset(), assignmentExpression.Source.GetTreeStartOffset());
      }
      else {
        ILocalVariable localVariable = localVariableDeclaration as ILocalVariable;
        if(localVariable == null) {
          return false;
        }

        IType declaredType = localVariable.Type;
        if(declaredType.GetPresentableName(localVariable.Language) != "string") {
          return false;
        }

        ILocalVariableDeclarationNode declNode = localVariableDeclaration.ToTreeNode();
        if(declNode.AssignmentSign == null) {
          return false;
        }

        _name = localVariable.ShortName;

        range = new TextRange(declNode.NameIdentifier.GetTreeStartOffset(), localVariableDeclaration.Initial.GetTreeStartOffset());
      }

      return (range.IsValid && range.Contains(Provider.CaretOffset));
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Inserts the assertion code.
    /// </summary>
    static void CheckStringAssignment(ILocalVariableDeclaration localVariableDeclaration) {
      ILocalVariable localVariable = localVariableDeclaration as ILocalVariable;
      if(localVariable == null) {
        return;
      }

      IStatement anchor = null;

      ITreeNode treeNode = localVariableDeclaration.ToTreeNode();

      while(treeNode != null) {
        anchor = treeNode as IStatement;

        if(anchor != null) {
          break;
        }

        treeNode = treeNode.Parent;
      }

      if(anchor == null) {
        return;
      }

      CheckStringAssignment(localVariableDeclaration, anchor, localVariable.ShortName);
    }

    /// <summary>
    /// Inserts the assertion code.
    /// </summary>
    /// <param name="assignmentExpression">The assignment expression.</param>
    static void CheckStringAssignment(IAssignmentExpression assignmentExpression) {
      ICSharpExpression destination = assignmentExpression.Dest;
      if(destination == null) {
        return;
      }

      if(!destination.IsClassifiedAsVariable) {
        return;
      }

      IType type = destination.GetExpressionType() as IType;
      if(type == null) {
        return;
      }

      IReferenceExpression referenceExpression = assignmentExpression.Dest as IReferenceExpression;
      if(referenceExpression == null) {
        return;
      }

      IStatement anchor = assignmentExpression.GetContainingStatement();

      CheckStringAssignment(assignmentExpression, anchor, referenceExpression.Reference.GetName());
    }

    /// <summary>
    /// Inserts the assert.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="anchor">The anchor.</param>
    /// <param name="name">The name.</param>
    static void CheckStringAssignment(IElement element, IStatement anchor, string name) {
      CodeFormatter codeFormatter = GetCodeFormatter();
      if(codeFormatter == null) {
        return;
      }

      IFunctionDeclaration functionDeclaration = anchor.GetContainingTypeMemberDeclaration() as IFunctionDeclaration;
      if(functionDeclaration == null) {
        return;
      }

      IBlock body = functionDeclaration.Body;
      if(body == null) {
        return;
      }
      CSharpElementFactory factory = CSharpElementFactory.GetInstance(element.GetProject());

      ICSharpElement csharpElement = element as ICSharpElement;
      if(csharpElement == null) {
        return;
      }

      string code = string.Format("if(string.IsNullOrEmpty({0})) {{ }}", name);

      IStatement statement = factory.CreateStatement(code);

      IStatement result = body.AddStatementAfter(statement, anchor);

      DocumentRange range = result.GetDocumentRange();
      IPsiRangeMarker marker = result.GetManager().CreatePsiRangeMarker(range);
      codeFormatter.Optimize(result.GetContainingFile(), marker, false, true, NullProgressIndicator.INSTANCE);
    }

    /// <summary>
    /// Gets the code formatter.
    /// </summary>
    /// <returns>The code formatter.</returns>
    [CanBeNull]
    static CodeFormatter GetCodeFormatter() {
      LanguageService languageService = LanguageServiceManager.Instance.GetLanguageService(CSharpLanguageService.CSHARP);
      if(languageService == null) {
        return null;
      }

      return languageService.CodeFormatter;
    }

    #endregion
  }
}