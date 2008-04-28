using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace AgentJohnson.ValueAnalysis {
  /// <summary>
  /// 
  /// </summary>
  [ConfigurableSeverityHighlighting(NAME)]
  public class ReturnWarning : SuggestionBase {

    #region Constants

    /// <summary>"ReturnAssertion"</summary>
    public const string NAME = "Return";

    #endregion

    #region Fields

    readonly ISolution _solution;
    readonly IReturnStatement _returnStatement;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ReturnWarning"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="returnStatement">The return statement.</param>
    public ReturnWarning(ISolution solution, IReturnStatement returnStatement): base(NAME, returnStatement, GetRange(returnStatement), "Return value should be asserted") {
      _solution = solution;
      _returnStatement = returnStatement;
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
    public IReturnStatement ReturnStatement {
      get {
        return _returnStatement;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="ReturnWarning"/> is enabled.
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