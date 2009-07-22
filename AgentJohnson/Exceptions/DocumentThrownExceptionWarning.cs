// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DocumentThrownExceptionWarning.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The document thrown exception warning.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Exceptions
{
  using JetBrains.DocumentModel;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Daemon;
  using JetBrains.ReSharper.Psi.CSharp.Tree;

  /// <summary>
  /// The document thrown exception warning.
  /// </summary>
  [ConfigurableSeverityHighlighting(NAME)]
  public class DocumentThrownExceptionWarning : SuggestionBase
  {
    #region Constants and Fields

    /// <summary>"UndocumentedThrownException"</summary>
    public const string NAME = "UndocumentedThrownException";

    /// <summary>
    /// The _solution.
    /// </summary>
    private readonly ISolution _solution;

    /// <summary>
    /// The _throw statement.
    /// </summary>
    private readonly IThrowStatement _throwStatement;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentThrownExceptionWarning"/> class.
    /// </summary>
    /// <param name="solution">
    /// The solution.
    /// </param>
    /// <param name="throwStatement">
    /// The throw statement.
    /// </param>
    public DocumentThrownExceptionWarning(ISolution solution, IThrowStatement throwStatement) : base(NAME, throwStatement, GetRange(throwStatement), "Thrown exception should be documented")
    {
      this._solution = solution;
      this._throwStatement = throwStatement;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether this <see cref="DocumentThrownExceptionWarning"/> is enabled.
    /// </summary>
    /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
    public static bool Enabled
    {
      get
      {
        return HighlightingSettingsManager.Instance.Settings.GetSeverity(NAME) != Severity.DO_NOT_SHOW;
      }
    }

    /// <summary>
    /// Get the severity of this highlighting
    /// </summary>
    /// <value></value>
    public override Severity Severity
    {
      get
      {
        var severity = HighlightingSettingsManager.Instance.Settings.GetSeverity(NAME);
        return severity == Severity.DO_NOT_SHOW ? severity : Severity.WARNING;
      }
    }

    /// <summary>
    /// Gets the solution.
    /// </summary>
    /// <value>The solution.</value>
    public ISolution Solution
    {
      get
      {
        return this._solution;
      }
    }

    /// <summary>
    /// Gets the statement.
    /// </summary>
    /// <value>The statement.</value>
    public IStatement ThrowStatement
    {
      get
      {
        return this._throwStatement;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Gets the range.
    /// </summary>
    /// <param name="statement">
    /// The statement.
    /// </param>
    /// <returns>
    /// </returns>
    private static DocumentRange GetRange(IStatement statement)
    {
      return statement.GetDocumentRange();
    }

    #endregion
  }
}