using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.ValueAnalysis {
  /// <summary>
  /// 
  /// </summary>
  [ConfigurableSeverityHighlighting(NAME)]
  public class ValueAnalysisSuggestion : SuggestionBase {

    #region Constants

    public const string NAME = "ValueAnalysis";

    #endregion

    #region Fields

    readonly ISolution _solution;
    readonly ITypeMemberDeclaration _typeMemberDeclaration;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueAnalysisSuggestion"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="typeMemberDeclaration">The type member declaration.</param>
    public ValueAnalysisSuggestion(ISolution solution, ITypeMemberDeclaration typeMemberDeclaration)
      : base(NAME, typeMemberDeclaration, typeMemberDeclaration.GetNameDocumentRange(), "Type members should be annotated with Value Analysis attributes.") {
      _solution = solution;
      _typeMemberDeclaration = typeMemberDeclaration;
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Attribute of this highlighting in the markup model
    /// </summary>
    /// <value></value>
    public override string AttributeId {
      get {
        return HighlightingAttributeIds.WARNING_ATTRIBUTE;
      }
    }

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
    /// Gets a value indicating whether this <see cref="ValueAnalysisSuggestion"/> is enabled.
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
  }
}