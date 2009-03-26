namespace AgentJohnson.Enums
{
  using System.Text;
  using JetBrains.Application.Progress;
  using JetBrains.DocumentModel;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CodeStyle;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.ReSharper.Psi.Util;

  /// <summary>
  /// Represents the Context Action.
  /// </summary>
  [ContextAction(Description = "Generates a 'switch' statement based on the current 'enum' expression.", Name = "Generate 'switch' statement", Priority = -1, Group = "C#")]
  public class GenerateSwitchContextAction : ContextActionBase
  {
    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateSwitchContextAction"/> class.
    /// </summary>
    /// <param name="provider">The provider.</param>
    public GenerateSwitchContextAction(ICSharpContextActionDataProvider provider) : base(provider)
    {
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes the internal.
    /// </summary>
    protected override void Execute(IElement element)
    {
      IExpressionStatement statement = this.GetSelectedElement<IExpressionStatement>(true);
      if (statement == null)
      {
        return;
      }

      CSharpElementFactory factory = CSharpElementFactory.GetInstance(statement.GetPsiModule());
      if (factory == null)
      {
        return;
      }

      ICSharpExpression expression = statement.Expression;
      if ((expression == null) || !(expression.ToTreeNode() is IUnaryExpressionNode))
      {
        return;
      }

      IType type = expression.Type();
      if (!type.IsResolved)
      {
        return;
      }

      IDeclaredType declaredType = type as IDeclaredType;
      if (declaredType == null)
      {
        return;
      }

      IEnum enumerate = declaredType.GetTypeElement() as IEnum;
      if (enumerate == null)
      {
        return;
      }

      string typeName = enumerate.ShortName;

      StringBuilder code = new StringBuilder("switch(");

      code.Append(statement.GetText());

      code.Append(") {");

      foreach (IField field in enumerate.EnumMembers)
      {
        code.Append("case ");
        code.Append(typeName);
        code.Append('.');
        code.Append(field.ShortName);
        code.Append(":\r\nbreak;");
      }

      code.Append("default:\r\n");
      code.Append("throw new System.ArgumentOutOfRangeException();");

      code.Append("\r\n}");

      IStatement result = factory.CreateStatement(code.ToString());
      if (result == null)
      {
        return;
      }

      result = statement.ReplaceBy(result);

      LanguageService languageService = LanguageServiceManager.Instance.GetLanguageService(CSharpLanguageService.CSHARP);
      if (languageService == null)
      {
        return;
      }

      CodeFormatter formatter = languageService.CodeFormatter;
      if (formatter == null)
      {
        return;
      }

      DocumentRange range = result.GetDocumentRange();
      IPsiRangeMarker marker = result.GetManager().CreatePsiRangeMarker(range);
      formatter.Optimize(result.GetContainingFile(), marker, false, true, NullProgressIndicator.Instance);
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    protected override string GetText()
    {
      return string.Format("Generate 'switch' statement");
    }

    /// <summary>
    /// Called to check if ContextAction is available.
    /// ReadLock is taken
    /// Will not be called if <c>PsiManager</c>, ProjectFile of Solution == null
    /// </summary>
    /// <returns></returns>
    protected override bool IsAvailable(IElement element)
    {
      IExpressionStatement statement = this.GetSelectedElement<IExpressionStatement>(true);
      if (statement == null)
      {
        return false;
      }

      ICSharpExpression expression = statement.Expression;
      if ((expression == null) || !(expression.ToTreeNode() is IUnaryExpressionNode))
      {
        return false;
      }

      IType type = expression.Type();
      if (!type.IsResolved)
      {
        return false;
      }

      return TypesUtil.IsEnumType(type);
    }

    #endregion
  }
}