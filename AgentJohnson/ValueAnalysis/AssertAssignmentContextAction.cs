namespace AgentJohnson.ValueAnalysis
{
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using JetBrains.Application;
  using JetBrains.Application.Progress;
  using JetBrains.CommonControls;
  using JetBrains.DocumentModel;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CodeStyle;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Resolve;
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
    #region Fields

    private string _name;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="AssertAssignmentContextAction"/> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    public AssertAssignmentContextAction(ICSharpContextActionDataProvider provider) : base(provider)
    {
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes the internal.
    /// </summary>
    protected override void Execute(IElement element)
    {
      if (!this.IsAvailable(element))
      {
        return;
      }

      IAssignmentExpression assignmentExpression = this.Provider.GetSelectedElement<IAssignmentExpression>(true, true);
      if (assignmentExpression != null)
      {
        InsertAssertionCode(assignmentExpression);
        return;
      }

      ILocalVariableDeclaration localVariableDeclaration = this.Provider.GetSelectedElement<ILocalVariableDeclaration>(true, true);
      if (localVariableDeclaration != null)
      {
        InsertAssertionCode(localVariableDeclaration);
        return;
      }
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    protected override string GetText()
    {
      return string.Format("Assert assignment to '{0}' [Agent Johnson]", this._name);
    }

    /// <summary>
    /// Called to check if ContextAction is available.
    /// ReadLock is taken
    /// Will not be called if <c>PsiManager</c>, ProjectFile of Solution == null
    /// </summary>
    /// <returns></returns>
    protected override bool IsAvailable(IElement element)
    {
      ILocalVariableDeclaration localVariableDeclaration = this.Provider.GetSelectedElement<ILocalVariableDeclaration>(true, true);
      IAssignmentExpression assignmentExpression = this.Provider.GetSelectedElement<IAssignmentExpression>(true, true);

      if (assignmentExpression == null && localVariableDeclaration == null)
      {
        return false;
      }

      TextRange range;
      IType declaredType;
      PsiLanguageType language;

      if (assignmentExpression != null)
      {
        ICSharpExpression destination = assignmentExpression.Dest;
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

        IReferenceExpression referenceExpression = destination as IReferenceExpression;
        if (referenceExpression == null)
        {
          return false;
        }

        IReference reference = referenceExpression.Reference;
        if (reference == null)
        {
          return false;
        }

        IExpression source = assignmentExpression.Source;
        if (source == null)
        {
          return false;
        }

        this._name = reference.GetName();

        range = new TextRange(destination.GetTreeStartOffset(), source.GetTreeStartOffset());
      }
      else
      {
        ILocalVariable localVariable = localVariableDeclaration as ILocalVariable;
        if (localVariable == null)
        {
          return false;
        }

        declaredType = localVariable.Type;
        language = localVariable.Language;

        ILocalVariableDeclarationNode declNode = localVariableDeclaration.ToTreeNode();
        if (declNode.AssignmentSign == null)
        {
          return false;
        }

        IVariableInitializer initial = localVariableDeclaration.Initial;
        if (initial == null)
        {
          return false;
        }

        IIdentifierNode identifier = declNode.NameIdentifier;
        if (identifier == null)
        {
          return false;
        }

        this._name = localVariable.ShortName;

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

      Rule rule = Rule.GetRule(declaredType, language) ?? Rule.GetDefaultRule();
      if (rule == null)
      {
        return false;
      }

      return rule.ValueAssertions.Count > 0;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Gets the code formatter.
    /// </summary>
    /// <returns>The code formatter.</returns>
    [CanBeNull]
    private static CodeFormatter GetCodeFormatter()
    {
      LanguageService languageService = LanguageServiceManager.Instance.GetLanguageService(CSharpLanguageService.CSHARP);
      if (languageService == null)
      {
        return null;
      }

      return languageService.CodeFormatter;
    }

    /// <summary>
    /// Inserts the assert.
    /// </summary>
    /// <param name="assertion">The assertion.</param>
    /// <param name="element">The element.</param>
    private static void InsertAssertionCode(string assertion, IElement element)
    {
      CodeFormatter codeFormatter = GetCodeFormatter();
      if (codeFormatter == null)
      {
        return;
      }

      IStatement anchor = null;
      string name;

      IAssignmentExpression assignmentExpression = element as IAssignmentExpression;
      if (assignmentExpression != null)
      {
        anchor = assignmentExpression.GetContainingStatement();

        IReferenceExpression referenceExpression = assignmentExpression.Dest as IReferenceExpression;
        if (referenceExpression == null)
        {
          return;
        }

        name = referenceExpression.Reference.GetName();
      }
      else
      {
        ITreeNode treeNode = element.ToTreeNode();

        while (treeNode != null)
        {
          anchor = treeNode as IStatement;

          if (anchor != null)
          {
            break;
          }

          treeNode = treeNode.Parent;
        }

        ILocalVariable localVariable = element as ILocalVariable;
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

      IMethodDeclaration functionDeclaration = anchor.GetContainingTypeMemberDeclaration() as IMethodDeclaration;
      if (functionDeclaration == null)
      {
        return;
      }

      IBlock body = functionDeclaration.Body;
      if (body == null)
      {
        return;
      }
      CSharpElementFactory factory = CSharpElementFactory.GetInstance(element.GetPsiModule());

      ICSharpElement csharpElement = element as ICSharpElement;
      if (csharpElement == null)
      {
        return;
      }

      string code = string.Format(assertion, name);

      IStatement statement = factory.CreateStatement(code);

      IStatement result = body.AddStatementAfter(statement, anchor);

      DocumentRange range = result.GetDocumentRange();
      IPsiRangeMarker marker = result.GetManager().CreatePsiRangeMarker(range);
      codeFormatter.Optimize(result.GetContainingFile(), marker, false, true, NullProgressIndicator.Instance);
    }

    /// <summary>
    /// Menu_s the item clicked.
    /// </summary>
    /// <param name="assertion">The assertion.</param>
    /// <param name="element">The element.</param>
    private void InsertAssertion(string assertion, IElement element)
    {
      PsiManager psiManager = PsiManager.GetInstance(this.Provider.Solution);
      if (psiManager == null)
      {
        return;
      }

      using (ReadLockCookie.Create())
      {
        using (ModificationCookie cookie = this.Provider.TextControl.Document.EnsureWritable())
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
    private void InsertAssertionCode(ILocalVariableDeclaration localVariableDeclaration)
    {
      ILocalVariable localVariable = localVariableDeclaration as ILocalVariable;
      if (localVariable == null)
      {
        return;
      }

      this.InsertAssertionCode(localVariable.Type, localVariableDeclaration, localVariable.ShortName);
    }

    /// <summary>
    /// Inserts the assertion code.
    /// </summary>
    /// <param name="assignmentExpression">The assignment expression.</param>
    private void InsertAssertionCode(IAssignmentExpression assignmentExpression)
    {
      ICSharpExpression destination = assignmentExpression.Dest;
      if (destination == null)
      {
        return;
      }

      if (!destination.IsClassifiedAsVariable)
      {
        return;
      }

      IType type = destination.GetExpressionType() as IType;
      if (type == null)
      {
        return;
      }

      IReferenceExpression referenceExpression = assignmentExpression.Dest as IReferenceExpression;
      if (referenceExpression == null)
      {
        return;
      }

      this.InsertAssertionCode(type, assignmentExpression, referenceExpression.Reference.GetName());
    }

    /// <summary>
    /// Inserts the assertion code.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="element">The element.</param>
    /// <param name="name">The name.</param>
    private void InsertAssertionCode(IType type, IElement element, string name)
    {
      Rule rule = Rule.GetRule(type, element.Language) ?? Rule.GetDefaultRule();
      if (rule == null)
      {
        return;
      }

      if (rule.ValueAssertions.Count == 1)
      {
        string valueAssertion = rule.ValueAssertions[0];

        InsertAssertionCode(valueAssertion, element);

        return;
      }

      this.ShowPopupMenu(element, rule, name);
    }

    /// <summary>
    /// Shows the popup menu.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="rule">The rule.</param>
    /// <param name="name">The name.</param>
    private void ShowPopupMenu(IElement element, Rule rule, string name)
    {
      JetPopupMenu menu = new JetPopupMenu();

      List<SimpleMenuItem> assertions = new List<SimpleMenuItem>(rule.ValueAssertions.Count);

      foreach (string valueAssertion in rule.ValueAssertions)
      {
        SimpleMenuItem item = new SimpleMenuItem
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