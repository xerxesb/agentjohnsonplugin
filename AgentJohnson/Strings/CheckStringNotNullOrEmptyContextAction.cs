// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckStringNotNullOrEmptyContextAction.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Represents the Context Action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Strings
{
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.DataProviders;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using Psi.CodeStyle;

  /// <summary>
  /// Represents the Context Action.
  /// </summary>
  [ContextAction(Description = "Adds an 'if' statement after the current statement that checks if the string variable is null or empty.", Name = "Check if string is null or empty", Priority = -1, Group = "C#")]
  public class CheckStringNotNullOrEmptyContextAction : ContextActionBase
  {
    #region Constants and Fields

    /// <summary>
    /// The Name field.
    /// </summary>
    private string name;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CheckStringNotNullOrEmptyContextAction"/> class.
    /// </summary>
    /// <param name="provider">
    /// The provider.
    /// </param>
    public CheckStringNotNullOrEmptyContextAction(ICSharpContextActionDataProvider provider) : base(provider)
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
        CheckStringAssignment(assignmentExpression);
        return;
      }

      var localVariableDeclaration = this.Provider.GetSelectedElement<ILocalVariableDeclaration>(true, true);
      if (localVariableDeclaration != null)
      {
        CheckStringAssignment(localVariableDeclaration);
        return;
      }
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <returns>
    /// The text to display.
    /// </returns>
    /// <value>
    /// The text that is shown in the context menu.
    /// </value>
    protected override string GetText()
    {
      return string.Format("Check if '{0}' is null or empty  [Agent Johnson]", this.name ?? "[unknown]");
    }

    /// <summary>
    /// Called to check if ContextAction is available.
    /// ReadLock is taken
    /// Will not be called if <c>PsiManager</c>, ProjectFile of Solution == <c>null</c>
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// Determines if the context action is available.
    /// </returns>
    protected override bool IsAvailable(IElement element)
    {
      this.name = null;

      var localVariableDeclaration = this.Provider.GetSelectedElement<ILocalVariableDeclaration>(true, true);
      var assignmentExpression = this.Provider.GetSelectedElement<IAssignmentExpression>(true, true);

      if (assignmentExpression == null && localVariableDeclaration == null)
      {
        return false;
      }

      global::JetBrains.Util.TextRange range;

      if (assignmentExpression != null)
      {
        var destination = assignmentExpression.Dest;
        if (destination == null)
        {
          return false;
        }

        IType declaredType = destination.GetExpressionType() as IDeclaredType;
        if (declaredType == null || declaredType.GetPresentableName(destination.Language) != "string")
        {
          return false;
        }

        if (!destination.IsClassifiedAsVariable)
        {
          return false;
        }

        var referenceExpression = destination as IReferenceExpression;
        if (referenceExpression == null)
        {
          return false;
        }

        var reference = referenceExpression.Reference;
        var source = assignmentExpression.Source;
        if (source == null)
        {
          return false;
        }

        this.name = reference.GetName();

        range = new global::JetBrains.Util.TextRange(destination.GetTreeStartOffset().Offset, source.GetTreeStartOffset().Offset);
      }
      else
      {
        var localVariable = localVariableDeclaration.DeclaredElement as ILocalVariable;
        if (localVariable == null)
        {
          return false;
        }

        var declaredType = localVariable.Type;
        if (declaredType.GetPresentableName(localVariable.Language) != "string")
        {
          return false;
        }

        var declNode = localVariableDeclaration.ToTreeNode();
        if (declNode.AssignmentSign == null)
        {
          return false;
        }

        this.name = localVariable.ShortName;

        var initial = localVariableDeclaration.Initial;
        if (initial == null)
        {
          return false;
        }

        range = new global::JetBrains.Util.TextRange(declNode.NameIdentifier.GetTreeStartOffset().Offset, initial.GetTreeStartOffset().Offset);
      }

      return range.IsValid() && range.Contains(this.Provider.CaretOffset.Offset);
    }

    /// <summary>
    /// Inserts the assertion code.
    /// </summary>
    /// <param name="localVariableDeclaration">
    /// The local variable declaration.
    /// </param>
    private void CheckStringAssignment(ILocalVariableDeclaration localVariableDeclaration)
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

      this.CheckStringAssignment(localVariableDeclaration, anchor, localVariable.ShortName);
    }

    /// <summary>
    /// Inserts the assertion code.
    /// </summary>
    /// <param name="assignmentExpression">
    /// The assignment expression.
    /// </param>
    private void CheckStringAssignment(IAssignmentExpression assignmentExpression)
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

      var referenceExpression = assignmentExpression.Dest as IReferenceExpression;
      if (referenceExpression == null)
      {
        return;
      }

      var anchor = assignmentExpression.GetContainingStatement();

      this.CheckStringAssignment(assignmentExpression, anchor, referenceExpression.Reference.GetName());
    }

    /// <summary>
    /// Inserts the assert.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="anchor">The anchor.</param>
    /// <param name="name">The name of the variable.</param>
    private void CheckStringAssignment(IElement element, IStatement anchor, string name)
    {
      var methodDeclaration = anchor.GetContainingTypeMemberDeclaration() as IMethodDeclaration;
      if (methodDeclaration == null)
      {
        return;
      }

      var body = methodDeclaration.Body;
      if (body == null)
      {
        return;
      }

      var csharpElement = element as ICSharpElement;
      if (csharpElement == null)
      {
        return;
      }

      var factory = CSharpElementFactory.GetInstance(element.GetPsiModule());

      var statement = factory.CreateStatement(string.Format("if (string.IsNullOrEmpty({0})) {{ }}", name));

      var result = body.AddStatementAfter(statement, anchor);

      var range = result.GetDocumentRange();
      var codeFormatter = new CodeFormatter();
      codeFormatter.Format(this.Solution, range);
    }

    #endregion
  }
}