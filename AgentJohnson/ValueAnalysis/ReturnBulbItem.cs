using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.TextControl;
using JetBrains.Util;

namespace AgentJohnson.ValueAnalysis {
  /// <summary>
  /// 
  /// </summary>
  public class ReturnBulbItem : IBulbItem {
    #region Fields

    readonly ReturnWarning _warning;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ReturnBulbItem"/> class.
    /// </summary>
    /// <param name="warning">The suggestion.</param>
    public ReturnBulbItem(ReturnWarning warning) {
      _warning = warning;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Executes the specified solution.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public void Execute(ISolution solution, ITextControl textControl) {
      PsiManager psiManager = PsiManager.GetInstance(solution);
      if(psiManager == null) {
        return;
      }

      using(ModificationCookie cookie = textControl.Document.EnsureWritable()) {
        if(cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS) {
          return;
        }

        using(CommandCookie.Create(string.Format("Context Action {0}", Text))) {
          psiManager.DoTransaction(delegate { Execute(); });
        }
      }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Executes this instance.
    /// </summary>
    void Execute() {
      IReturnStatement returnStatement = _warning.ReturnStatement;

      IFunction function = returnStatement.GetContainingTypeMemberDeclaration() as IFunction;
      if(function == null) {
        return;
      }

      IType type = function.ReturnType;

      Rule rule = Rule.GetRule(type, function.Language) ?? Rule.GetDefaultRule();
      if(rule == null) {
        return;
      }

      CodeFormatter codeFormatter = GetCodeFormatter();
      if(codeFormatter == null) {
        return;
      }

      string code = rule.ReturnAssertion;

      string expression = returnStatement.Value.GetText();
      string typeName = type.GetLongPresentableName(returnStatement.Language);

      code = "return " + string.Format(code, expression, typeName) + ";";

      CSharpElementFactory factory = CSharpElementFactory.GetInstance(returnStatement.GetProject());

      IStatement statement = factory.CreateStatement(code);

      IStatement result = returnStatement.ReplaceBy(statement);

      DocumentRange range = result.GetDocumentRange();
      IPsiRangeMarker marker = result.GetManager().CreatePsiRangeMarker(range);
      codeFormatter.Optimize(result.GetContainingFile(), marker, false, true, NullProgressIndicator.INSTANCE);
    }

    /// <summary>
    /// Gets the code formatter.
    /// </summary>
    /// <returns></returns>
    static CodeFormatter GetCodeFormatter() {
      LanguageService languageService = LanguageServiceManager.Instance.GetLanguageService(CSharpLanguageService.CSHARP);
      if(languageService == null) {
        return null;
      }

      return languageService.CodeFormatter;
    }

    #endregion
                                
    #region IBulbItem Members

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text {
      get {
        return "Assert return value";
      }
    }

    #endregion
  }
}