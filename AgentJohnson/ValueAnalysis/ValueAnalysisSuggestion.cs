// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueAnalysisSuggestion.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The value analysis suggestion.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.ValueAnalysis
{
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Daemon;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// The value analysis suggestion.
  /// </summary>
  [ConfigurableSeverityHighlighting(NAME)]
  public class ValueAnalysisSuggestion : SuggestionBase
  {
    #region Constants and Fields

    /// <summary>
    /// The name.
    /// </summary>
    public const string NAME = "ValueAnalysis";

    /// <summary>
    /// The _solution.
    /// </summary>
    private readonly ISolution _solution;

    /// <summary>
    /// The _type member declaration.
    /// </summary>
    private readonly ITypeMemberDeclaration _typeMemberDeclaration;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueAnalysisSuggestion"/> class.
    /// </summary>
    /// <param name="solution">
    /// The solution.
    /// </param>
    /// <param name="typeMemberDeclaration">
    /// The type member declaration.
    /// </param>
    public ValueAnalysisSuggestion(ISolution solution, ITypeMemberDeclaration typeMemberDeclaration)
      : base(NAME, typeMemberDeclaration, typeMemberDeclaration.GetNameDocumentRange(), "Type members should be annotated with Value Analysis attributes.")
    {
      this._solution = solution;
      this._typeMemberDeclaration = typeMemberDeclaration;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether this <see cref="ValueAnalysisSuggestion"/> is enabled.
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

    #endregion
  }
}