using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace AgentJohnson.Exceptions {
  /// <summary>
  /// 
  /// </summary>
  [ConfigurableSeverityHighlighting(NAME)]
  public class DocumentThrownExceptionWarning : SuggestionBase {

    #region Constants

    /// <summary>"UndocumentedThrownException"</summary>
    public const string NAME = "UndocumentedThrownException";

    #endregion

    #region Fields

    readonly ISolution _solution;
    readonly IThrowStatement _throwStatement;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentThrownExceptionWarning"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="throwStatement">The throw statement.</param>
    public DocumentThrownExceptionWarning(ISolution solution, IThrowStatement throwStatement): base(NAME, throwStatement, GetRange(throwStatement), "Thrown exception should be documented") {
      _solution = solution;
      _throwStatement = throwStatement;
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the solution.
    /// </summary>
    /// <value>The solution.</value>
    public ISolution Solution {
      get {
        return _solution;
      }
    }

    /// <summary>
    /// Gets the statement.
    /// </summary>
    /// <value>The statement.</value>
    public IStatement ThrowStatement {
      get {
        return _throwStatement;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="DocumentThrownExceptionWarning"/> is enabled.
    /// </summary>
    /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
    public static bool Enabled {
      get {
        return HighlightingSettingsManager.Instance.Settings.GetSeverity(NAME) != Severity.DO_NOT_SHOW;
      }
    }

    /// <summary>
    /// Get the severity of this highlighting
    /// </summary>
    /// <value></value>
    public override Severity Severity {
      get {
        Severity severity = HighlightingSettingsManager.Instance.Settings.GetSeverity(NAME);
        return severity == Severity.DO_NOT_SHOW ? severity : Severity.WARNING;
      }
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Gets the range.
    /// </summary>
    /// <param name="statement">The statement.</param>
    /// <returns></returns>
    static DocumentRange GetRange(IStatement statement) {
      return statement.GetDocumentRange();
    }

    #endregion
  }
}