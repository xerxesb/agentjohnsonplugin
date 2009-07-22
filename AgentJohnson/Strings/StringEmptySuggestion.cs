// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringEmptySuggestion.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the string empty suggestion class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Strings
{
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Daemon;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Defines the string empty suggestion class.
  /// </summary>
  [ConfigurableSeverityHighlighting(NAME)]
  public class StringEmptySuggestion : SuggestionBase
  {
    #region Constants and Fields

    /// <summary>
    /// "StringEmpty" text.
    /// </summary>
    public const string NAME = "StringEmpty";

    /// <summary>
    /// The _node.
    /// </summary>
    private readonly ITokenNode _node;

    /// <summary>
    /// The _solution.
    /// </summary>
    private readonly ISolution _solution;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StringEmptySuggestion"/> class.
    /// </summary>
    /// <param name="solution">
    /// The solution.
    /// </param>
    /// <param name="node">
    /// The node.
    /// </param>
    public StringEmptySuggestion(ISolution solution, ITokenNode node) : base(NAME, node, node.GetDocumentRange(), "Empty string literals (\"\") should be string.Empty")
    {
      this._solution = solution;
      this._node = node;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether this <see cref="StringEmptySuggestion"/> is enabled.
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
    /// Gets the node.
    /// </summary>
    /// <value>The node.</value>
    public ITokenNode Node
    {
      get
      {
        return this._node;
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
        return severity == Severity.DO_NOT_SHOW ? severity : Severity.SUGGESTION;
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
  }
}