// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckAssignmentContextAction.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Represents the Context Action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.ValueAnalysis
{
  using JetBrains.Annotations;
  using JetBrains.Application.Progress;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CodeStyle;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.ReSharper.Psi.Util;
  using JetBrains.Util;

  /// <summary>
  /// Represents the Context Action.
  /// </summary>
  [ContextAction(Description = "Adds an 'if' statement after the current statement that checks if the variable is null.", Name = "Check if variable is null", Priority = -1, Group = "C#")]
  public class CheckAssignmentContextAction : ContextActionBase
  {
    #region Constants and Fields

    /// <summary>
    /// The _name.
    /// </summary>
    private string _name;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckAssignmentContextAction"/> class.
    /// </summary>
    /// <param name="provider">
    /// The provider.
    /// </param>
    public CheckAssignmentContextAction(ICSharpContextActionDataProvider provider) : base(provider)
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Executes the internal.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    protected override void Execute(IElement element)
    {
      if (!this.IsAvailable(element))
      {
        return;
      }

      var assignmentExpression = this.Provider.GetSelectedElement<IAssignmentExpression>(true, true);
      if (assignmentExpression != null)
      {
        CheckAssignment(assignmentExpression);
        return;
      }

      var localVariableDeclaration = this.Provider.GetSelectedElement<ILocalVariableDeclaration>(true, true);
      if (localVariableDeclaration != null)
      {
        CheckAssignment(localVariableDeclaration);
        return;
      }
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    /// <returns>
    /// The get text.
    /// </returns>
    protected override string GetText()
    {
      return string.Format("Check if '{0}' is null [Agent Johnson]", this._name ?? "[unknown]");
    }

    /// <summary>
    /// Called to check if ContextAction is available.
    /// ReadLock is taken
    /// Will not be called if <c>PsiManager</c>, ProjectFile of Solution == null
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// The is available.
    /// </returns>
    protected override bool IsAvailable(IElement element)
    {
      this._name = null;

      var localVariableDeclaration = this.Provider.GetSelectedElement<ILocalVariableDeclaration>(true, true);
      var assignmentExpression = this.Provider.GetSelectedElement<IAssignmentExpression>(true, true);

      if (assignmentExpression == null && localVariableDeclaration == null)
      {
        return false;
      }

      TextRange range;
      IType declaredType;

      if (assignmentExpression != null)
      {
        var destination = assignmentExpression.Dest;
        if (destination == null)
        {
          return false;
        }

        if (!destination.IsClassifiedAsVariable)
        {
          return false;
        }

        declaredType = destination.GetExpressionType() as IDeclaredType;

        var referenceExpression = destination as IReferenceExpression;
        if (referenceExpression == null)
        {
          return false;
        }

        var reference = referenceExpression.Reference;
        if (reference == null)
        {
          return false;
        }

        var source = assignmentExpression.Source;
        if (source == null)
        {
          return false;
        }

        this._name = reference.GetName();

        range = new TextRange(destination.GetTreeStartOffset(), source.GetTreeStartOffset());
      }
      else
      {
        var localVariable = localVariableDeclaration.DeclaredElement as ILocalVariable;
        if (localVariable == null)
        {
          return false;
        }

        declaredType = localVariable.Type;

        var declNode = localVariableDeclaration.ToTreeNode();
        if (declNode.AssignmentSign == null)
        {
          return false;
        }

        this._name = localVariable.ShortName;

        IIdentifierNode identifier = declNode.NameIdentifier;
        if (identifier == null)
        {
          return false;
        }

        var initial = localVariableDeclaration.Initial;
        if (initial == null)
        {
          return false;
        }

        range = new TextRange(identifier.GetTreeStartOffset(), initial.GetTreeStartOffset());
      }

      if (declaredType == null)
      {
        return false;
      }

      if (!declaredType.IsReferenceType())
      {
        return false;
      }

      return range.IsValid() && range.Contains(this.Provider.CaretOffset);
    }

    /// <summary>
    /// Inserts the assertion code.
    /// </summary>
    /// <param name="localVariableDeclaration">
    /// The local variable declaration.
    /// </param>
    private static void CheckAssignment(ILocalVariableDeclaration localVariableDeclaration)
    {
      var localVariable = localVariableDeclaration.DeclaredElement as ILocalVariable;
      if (localVariable == null)
      {
        return;
      }

      IStatement anchor = null;

      ITreeNode treeNode = localVariableDeclaration.ToTreeNode();

      while (treeNode != null)
      {
        anchor = treeNode as IStatement;

        if (anchor != null)
        {
          break;
        }

        treeNode = treeNode.Parent;
      }

      if (anchor == null)
      {
        return;
      }

      CheckAssignment(localVariableDeclaration, anchor, localVariable.ShortName);
    }

    /// <summary>
    /// Inserts the assertion code.
    /// </summary>
    /// <param name="assignmentExpression">
    /// The assignment expression.
    /// </param>
    private static void CheckAssignment(IAssignmentExpression assignmentExpression)
    {
      var destination = assignmentExpression.Dest;
      if (destination == null)
      {
        return;
      }

      if (!destination.IsClassifiedAsVariable)
      {
        return;
      }

      var type = destination.GetExpressionType() as IType;
      if (type == null)
      {
        return;
      }

      var referenceExpression = assignmentExpression.Dest as IReferenceExpression;
      if (referenceExpression == null)
      {
        return;
      }

      var anchor = assignmentExpression.GetContainingStatement();

      CheckAssignment(assignmentExpression, anchor, referenceExpression.Reference.GetName());
    }

    /// <summary>
    /// Inserts the assert.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <param name="anchor">
    /// The anchor.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    private static void CheckAssignment(IElement element, IStatement anchor, string name)
    {
      var codeFormatter = GetCodeFormatter();
      if (codeFormatter == null)
      {
        return;
      }

      var functionDeclaration = anchor.GetContainingTypeMemberDeclaration() as IMethodDeclaration;
      if (functionDeclaration == null)
      {
        return;
      }

      var body = functionDeclaration.Body;
      if (body == null)
      {
        return;
      }

      var factory = CSharpElementFactory.GetInstance(element.GetPsiModule());

      var csharpElement = element as ICSharpElement;
      if (csharpElement == null)
      {
        return;
      }

      var code = string.Format("if({0} == null) {{ }}", name);

      var statement = factory.CreateStatement(code);

      var result = body.AddStatementAfter(statement, anchor);

      var range = result.GetDocumentRange();
      var marker = result.GetManager().CreatePsiRangeMarker(range);
      codeFormatter.Optimize(result.GetContainingFile(), marker, false, true, NullProgressIndicator.Instance);
    }

    /// <summary>
    /// Gets the code formatter.
    /// </summary>
    /// <returns>
    /// The code formatter.
    /// </returns>
    [CanBeNull]
    private static CodeFormatter GetCodeFormatter()
    {
      var languageService = LanguageServiceManager.Instance.GetLanguageService(CSharpLanguageService.CSHARP);
      if (languageService == null)
      {
        return null;
      }

      return languageService.CodeFormatter;
    }

    #endregion
  }
}