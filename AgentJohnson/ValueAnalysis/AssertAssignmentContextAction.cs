// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssertAssignmentContextAction.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Represents the Context Action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.ValueAnalysis
{
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using JetBrains.Application;
  using JetBrains.Application.Progress;
  using JetBrains.CommonControls;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CodeStyle;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.ReSharper.Psi.Util;
  using JetBrains.UI.PopupMenu;
  using JetBrains.Util;

  /// <summary>
  /// Represents the Context Action.
  /// </summary>
  [ContextAction(Description = "Adds an assertion statement after the current statement.", Name = "Assert assignment", Priority = 0, Group = "C#")]
  public class AssertAssignmentContextAction : ContextActionBase
  {
    #region Constants and Fields

    /// <summary>
    /// The name.
    /// </summary>
    private string name;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertAssignmentContextAction"/> class.
    /// </summary>
    /// <param name="provider">
    /// The provider.
    /// </param>
    public AssertAssignmentContextAction(ICSharpContextActionDataProvider provider) : base(provider)
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
        InsertAssertionCode(assignmentExpression);
        return;
      }

      var localVariableDeclaration = this.Provider.GetSelectedElement<ILocalVariableDeclaration>(true, true);
      if (localVariableDeclaration != null)
      {
        InsertAssertionCode(localVariableDeclaration);
        return;
      }
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <returns>
    /// The context action text.
    /// </returns>
    /// <value>
    /// The context action text.
    /// </value>
    protected override string GetText()
    {
      return string.Format("Assert assignment to '{0}' [Agent Johnson]", this.name);
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
    /// <c>true</c> if this instance is available; otherwise, <c>false</c>.
    /// </returns>
    protected override bool IsAvailable(IElement element)
    {
      var localVariableDeclaration = this.Provider.GetSelectedElement<ILocalVariableDeclaration>(true, true);
      var assignmentExpression = this.Provider.GetSelectedElement<IAssignmentExpression>(true, true);

      if (assignmentExpression == null && localVariableDeclaration == null)
      {
        return false;
      }

      TextRange range;
      IType declaredType;
      PsiLanguageType language;

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
        language = destination.Language;

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

        IExpression source = assignmentExpression.Source;
        if (source == null)
        {
          return false;
        }

        this.name = reference.GetName();

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
        language = localVariable.Language;

        var declNode = localVariableDeclaration.ToTreeNode();
        if (declNode.AssignmentSign == null)
        {
          return false;
        }

        var initial = localVariableDeclaration.Initial;
        if (initial == null)
        {
          return false;
        }

        IIdentifierNode identifier = declNode.NameIdentifier;
        if (identifier == null)
        {
          return false;
        }

        this.name = localVariable.ShortName;

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

      if (!range.IsValid() || !range.Contains(this.Provider.CaretOffset))
      {
        return false;
      }

      var rule = Rule.GetRule(declaredType, language) ?? Rule.GetDefaultRule();
      if (rule == null)
      {
        return false;
      }

      return rule.ValueAssertions.Count > 0;
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

    /// <summary>
    /// Inserts the assert.
    /// </summary>
    /// <param name="assertion">
    /// The assertion.
    /// </param>
    /// <param name="element">
    /// The element.
    /// </param>
    private static void InsertAssertionCode(string assertion, IElement element)
    {
      var codeFormatter = GetCodeFormatter();
      if (codeFormatter == null)
      {
        return;
      }

      IStatement anchor = null;
      string name;

      var assignmentExpression = element as IAssignmentExpression;
      if (assignmentExpression != null)
      {
        anchor = assignmentExpression.GetContainingStatement();

        var referenceExpression = assignmentExpression.Dest as IReferenceExpression;
        if (referenceExpression == null)
        {
          return;
        }

        name = referenceExpression.Reference.GetName();
      }
      else
      {
        var treeNode = element.ToTreeNode();

        while (treeNode != null)
        {
          anchor = treeNode as IStatement;

          if (anchor != null)
          {
            break;
          }

          treeNode = treeNode.Parent;
        }

        var localVariable = element as ILocalVariable;
        if (localVariable == null)
        {
          return;
        }

        name = localVariable.ShortName;
      }

      if (anchor == null)
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

      var code = string.Format(assertion, name);

      var statement = factory.CreateStatement(code);

      var result = body.AddStatementAfter(statement, anchor);

      var range = result.GetDocumentRange();
      var marker = result.GetManager().CreatePsiRangeMarker(range);
      codeFormatter.Optimize(result.GetContainingFile(), marker, false, true, NullProgressIndicator.Instance);
    }

    /// <summary>
    /// Menu_s the item clicked.
    /// </summary>
    /// <param name="assertion">
    /// The assertion.
    /// </param>
    /// <param name="element">
    /// The element.
    /// </param>
    private void InsertAssertion(string assertion, IElement element)
    {
      var psiManager = PsiManager.GetInstance(this.Provider.Solution);
      if (psiManager == null)
      {
        return;
      }

      using (ReadLockCookie.Create())
      {
        using (var cookie = this.Provider.TextControl.Document.EnsureWritable())
        {
          if (cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS)
          {
            return;
          }

          using (CommandCookie.Create(string.Format("Context Action {0}", this.GetText())))
          {
            psiManager.DoTransaction(delegate { InsertAssertionCode(assertion, element); });
          }
        }
      }
    }

    /// <summary>
    /// Inserts the assertion code.
    /// </summary>
    /// <param name="localVariableDeclaration">
    /// The local variable declaration.
    /// </param>
    private void InsertAssertionCode(ILocalVariableDeclaration localVariableDeclaration)
    {
      var localVariable = localVariableDeclaration.DeclaredElement as ILocalVariable;
      if (localVariable == null)
      {
        return;
      }

      this.InsertAssertionCode(localVariable.Type, localVariableDeclaration, localVariable.ShortName);
    }

    /// <summary>
    /// Inserts the assertion code.
    /// </summary>
    /// <param name="assignmentExpression">
    /// The assignment expression.
    /// </param>
    private void InsertAssertionCode(IAssignmentExpression assignmentExpression)
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

      this.InsertAssertionCode(type, assignmentExpression, referenceExpression.Reference.GetName());
    }

    /// <summary>
    /// Inserts the assertion code.
    /// </summary>
    /// <param name="type">
    /// The type.
    /// </param>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    private void InsertAssertionCode(IType type, IElement element, string name)
    {
      var rule = Rule.GetRule(type, element.Language) ?? Rule.GetDefaultRule();
      if (rule == null)
      {
        return;
      }

      if (rule.ValueAssertions.Count == 1)
      {
        var valueAssertion = rule.ValueAssertions[0];

        InsertAssertionCode(valueAssertion, element);

        return;
      }

      this.ShowPopupMenu(element, rule, name);
    }

    /// <summary>
    /// Shows the popup menu.
    /// </summary>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <param name="rule">
    /// The rule.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    private void ShowPopupMenu(IElement element, Rule rule, string name)
    {
      var menu = new JetPopupMenu();

      var assertions = new List<SimpleMenuItem>(rule.ValueAssertions.Count);

      foreach (var valueAssertion in rule.ValueAssertions)
      {
        var item = new SimpleMenuItem
        {
          Text = string.Format(valueAssertion, name),
          Style = MenuItemStyle.Enabled
        };

        item.Clicked += delegate { this.InsertAssertion(item.Text, element); };

        assertions.Add(item);
      }

      menu.Caption.Value = WindowlessControl.Create("Assert Assignment");
      menu.SetItems(assertions.ToArray());
      menu.KeyboardAcceleration.SetValue(KeyboardAccelerationFlags.Mnemonics);

      menu.Show();
    }

    #endregion
  }
}