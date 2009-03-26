namespace AgentJohnson.ValueAnalysis
{
  using JetBrains.Application;
  using JetBrains.Application.Progress;
  using JetBrains.DocumentModel;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Feature.Services.Bulbs;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CodeStyle;
  using JetBrains.ReSharper.Psi.CSharp;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.TextControl;
  using JetBrains.Util;

  /// <summary>
  /// Defines the return bulb item class.
  /// </summary>
  public class ReturnBulbItem : IBulbItem
  {
    #region Fields

    private readonly ReturnWarning _warning;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ReturnBulbItem"/> class.
    /// </summary>
    /// <param name="warning">The suggestion.</param>
    public ReturnBulbItem(ReturnWarning warning)
    {
      this._warning = warning;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Executes the specified solution.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public void Execute(ISolution solution, ITextControl textControl)
    {
      PsiManager psiManager = PsiManager.GetInstance(solution);
      if (psiManager == null)
      {
        return;
      }

      using (ModificationCookie cookie = textControl.Document.EnsureWritable())
      {
        if (cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS)
        {
          return;
        }

        using (CommandCookie.Create(string.Format("Context Action {0}", this.Text)))
        {
          psiManager.DoTransaction(delegate { this.Execute(); });
        }
      }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Gets the code formatter.
    /// </summary>
    /// <returns></returns>
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
    /// Executes this instance.
    /// </summary>
    private void Execute()
    {
      IReturnStatement returnStatement = this._warning.ReturnStatement;

      IFunction function = returnStatement.GetContainingTypeMemberDeclaration() as IFunction;
      if (function == null)
      {
        return;
      }

      IType type = function.ReturnType;

      Rule rule = Rule.GetRule(type, function.Language) ?? Rule.GetDefaultRule();
      if (rule == null)
      {
        return;
      }

      CodeFormatter codeFormatter = GetCodeFormatter();
      if (codeFormatter == null)
      {
        return;
      }

      string code = rule.ReturnAssertion;

      string expression = returnStatement.Value.GetText();
      string typeName = type.GetLongPresentableName(returnStatement.Language);

      code = "return " + string.Format(code, expression, typeName) + ";";

      CSharpElementFactory factory = CSharpElementFactory.GetInstance(returnStatement.GetPsiModule());

      IStatement statement = factory.CreateStatement(code);

      IStatement result = returnStatement.ReplaceBy(statement);

      DocumentRange range = result.GetDocumentRange();
      IPsiRangeMarker marker = result.GetManager().CreatePsiRangeMarker(range);
      codeFormatter.Optimize(result.GetContainingFile(), marker, false, true, NullProgressIndicator.Instance);
    }

    #endregion

    #region IBulbItem Members

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text
    {
      get
      {
        return "Assert return value";
      }
    }

    #endregion
  }
}