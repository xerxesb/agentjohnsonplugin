using System.Text;
using JetBrains.Application.Progress;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Intentions.CSharp.ContextActions.Util;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CodeStyle;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.TextControl;

namespace AgentJohnson.Enums {
  /// <summary>
  /// Represents the Context Action.
  /// </summary>
  [ContextAction(Description = "Generates a 'switch' statement based on the current 'enum' expression.", Name = "Generate 'switch' statement", Priority = -1, Group = "C#")]
  public class GenerateSwitchContextAction : OneItemContextActionBase {
    #region Fields

    readonly ISolution _solution;
    readonly ITextControl _textControl;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateSwitchContextAction"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public GenerateSwitchContextAction(ISolution solution, ITextControl textControl) : base(solution, textControl) {
      _solution = solution;
      _textControl = textControl;
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    public override string Text {
      get {
        return string.Format("Generate 'switch' statement");
      }
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes the internal.
    /// </summary>
    protected override void ExecuteInternal() {
      IExpressionStatement statement = GetSelectedElement<IExpressionStatement>(true);
      if(statement == null) {
        return;
      }

      CSharpElementFactory factory = CSharpElementFactory.GetInstance(statement.GetProject());
      if(factory == null) {
        return;
      }

      ICSharpExpression expression = statement.Expression;
      if((expression == null) || !(expression.ToTreeNode() is IUnaryExpressionNode)) {
        return;
      }

      IType type = expression.Type();
      if(!type.IsResolved) {
        return;
      }

      IDeclaredType declaredType = type as IDeclaredType;
      if(declaredType == null) {
        return;
      }

      IEnum enumerate = declaredType.GetTypeElement() as IEnum;
      if(enumerate == null) {
        return;
      }

      string typeName = enumerate.ShortName;

      StringBuilder code = new StringBuilder("switch(");

      code.Append(statement.GetText());

      code.Append(") {");

      foreach(IField field in enumerate.EnumMembers) {
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
      if(result == null) {
        return;
      }

      result = statement.ReplaceBy(result);

      LanguageService languageService = LanguageServiceManager.Instance.GetLanguageService(CSharpLanguageService.CSHARP);
      if(languageService == null) {
        return;
      }

      CodeFormatter formatter = languageService.CodeFormatter;
      if(formatter == null) {
        return;
      }

      DocumentRange range = result.GetDocumentRange();
      IPsiRangeMarker marker = result.GetManager().CreatePsiRangeMarker(range);
      formatter.Optimize(result.GetContainingFile(), marker, false, true, NullProgressIndicator.INSTANCE);
    }

    /// <summary>
    /// Called to check if ContextAction is available.
    /// ReadLock is taken
    /// Will not be called if <c>PsiManager</c>, ProjectFile of Solution == null
    /// </summary>
    /// <returns></returns>
    protected override bool IsAvailableInternal() {
      IExpressionStatement statement = GetSelectedElement<IExpressionStatement>(true);
      if(statement == null) {
        return false;
      }
      
      ICSharpExpression expression = statement.Expression;
      if((expression == null) || !(expression.ToTreeNode() is IUnaryExpressionNode)) {
        return false;
      }

      IType type = expression.Type();
      if(!type.IsResolved) {
        return false;
      }

      return MiscUtil.IsEnumType(type);
    }

    #endregion

  }
}