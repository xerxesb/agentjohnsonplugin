// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReturnWarning.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The return warning.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.ValueAnalysis
{
  using JetBrains.DocumentModel;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Daemon;
  using JetBrains.ReSharper.Psi.CSharp.Tree;

  /// <summary>
  /// The return warning.
  /// </summary>
  [ConfigurableSeverityHighlighting(NAME)]
  public class ReturnWarning : SuggestionBase
  {
    #region Constants and Fields

    /// <summary>"ReturnAssertion"</summary>
    public const string NAME = "Return";

    /// <summary>
    /// The _return statement.
    /// </summary>
    private readonly IReturnStatement _returnStatement;

    /// <summary>
    /// The _solution.
    /// </summary>
    private readonly ISolution _solution;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ReturnWarning"/> class.
    /// </summary>
    /// <param name="solution">
    /// The solution.
    /// </param>
    /// <param name="returnStatement">
    /// The return statement.
    /// </param>
    public ReturnWarning(ISolution solution, IReturnStatement returnStatement) : base(NAME, returnStatement, GetRange(returnStatement), "Return value should be asserted")
    {
      this._solution = solution;
      this._returnStatement = returnStatement;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether this <see cref="ReturnWarning"/> is enabled.
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
    /// Gets the statement.
    /// </summary>
    /// <value>The statement.</value>
    public IReturnStatement ReturnStatement
    {
      get
      {
        return this._returnStatement;
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