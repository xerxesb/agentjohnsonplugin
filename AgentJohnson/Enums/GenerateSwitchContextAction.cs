// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenerateSwitchContextAction.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Represents the Context Action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Enums
{
  using System.Text;
  using JetBrains.Application.Progress;
  using JetBrains.ReSharper.Intentions;
  using JetBrains.ReSharper.Intentions.CSharp.ContextActions;
  using JetBrains.ReSharper.Psi;
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
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateSwitchContextAction"/> class.
    /// </summary>
    /// <param name="provider">
    /// The provider.
    /// </param>
    public GenerateSwitchContextAction(ICSharpContextActionDataProvider provider) : base(provider)
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
      var statement = this.GetSelectedElement<IExpressionStatement>(true);
      if (statement == null)
      {
        return;
      }

      var factory = CSharpElementFactory.GetInstance(statement.GetPsiModule());
      if (factory == null)
      {
        return;
      }

      var expression = statement.Expression;
      if ((expression == null) || !(expression.ToTreeNode() is IUnaryExpressionNode))
      {
        return;
      }

      var type = expression.Type();
      if (!type.IsResolved)
      {
        return;
      }

      var declaredType = type as IDeclaredType;
      if (declaredType == null)
      {
        return;
      }

      var enumerate = declaredType.GetTypeElement() as IEnum;
      if (enumerate == null)
      {
        return;
      }

      var typeName = enumerate.ShortName;

      var code = new StringBuilder("switch(");

      code.Append(statement.GetText());

      code.Append(") {");

      foreach (var field in enumerate.EnumMembers)
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

      var result = factory.CreateStatement(code.ToString());
      if (result == null)
      {
        return;
      }

      result = statement.ReplaceBy(result);

      var languageService = LanguageServiceManager.Instance.GetLanguageService(CSharpLanguageService.CSHARP);
      if (languageService == null)
      {
        return;
      }

      var formatter = languageService.CodeFormatter;
      if (formatter == null)
      {
        return;
      }

      var range = result.GetDocumentRange();
      var marker = result.GetManager().CreatePsiRangeMarker(range);
      formatter.Optimize(result.GetContainingFile(), marker, false, true, NullProgressIndicator.Instance);
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
      return string.Format("Generate 'switch' statement [Agent Johnson]");
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
      var statement = this.GetSelectedElement<IExpressionStatement>(true);
      if (statement == null)
      {
        return false;
      }

      var expression = statement.Expression;
      if ((expression == null) || !(expression.ToTreeNode() is IUnaryExpressionNode))
      {
        return false;
      }

      var type = expression.Type();
      if (!type.IsResolved)
      {
        return false;
      }

      return TypesUtil.IsEnumType(type);
    }

    #endregion
  }
}