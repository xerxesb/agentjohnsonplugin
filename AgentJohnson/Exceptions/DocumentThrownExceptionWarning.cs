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
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// The document thrown exception warning.
  /// </summary>
  [ConfigurableSeverityHighlighting(Name)]
  public class DocumentThrownExceptionWarning : SuggestionBase
  {
    #region Constants and Fields

    /// <summary>"UndocumentedThrownException"</summary>
    public const string Name = "UndocumentedThrownException";

    /// <summary>
    /// The _throw statement.
    /// </summary>
    private readonly IThrowStatement throwStatement;

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
    public DocumentThrownExceptionWarning(IThrowStatement throwStatement) : base(Name, throwStatement, GetRange(throwStatement), "Thrown exception should be documented [Agent Johnson]")
    {
      this.throwStatement = throwStatement;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Get the severity of this highlighting
    /// </summary>
    /// <value></value>
    public override Severity Severity
    {
      get
      {
        var severity = HighlightingSettingsManager.Instance.Settings.GetSeverity(Name);
        return severity == Severity.DO_NOT_SHOW ? severity : Severity.WARNING;
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
        return this.throwStatement;
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