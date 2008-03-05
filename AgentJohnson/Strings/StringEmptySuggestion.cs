using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.Strings {
  /// <summary>
  /// 
  /// </summary>
  [ConfigurableSeverityHighlighting(NAME)]
  public class StringEmptySuggestion : SuggestionBase {

    #region Constants

    public const string NAME = "StringEmpty";

    #endregion

    #region Fields

    readonly ISolution _solution;
    readonly ITokenNode _node;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StringEmptySuggestion"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="node">The node.</param>
    public StringEmptySuggestion(ISolution solution, ITokenNode node): base(NAME, node, node.GetDocumentRange(), "Empty string literals (\"\") should be string.Empty") {
      _solution = solution;
      _node = node;
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Attribute of this highlighting in the markup model
    /// </summary>
    /// <value></value>
    public override string AttributeId {
      get {
        return HighlightingAttributeIds.GetDefaultAttribute(Severity.SUGGESTION);
      }
    }

    /// <summary>
    /// Gets the node.
    /// </summary>
    /// <value>The node.</value>
    public ITokenNode Node {
      get {
        return _node;
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
    /// Gets a value indicating whether this <see cref="StringEmptySuggestion"/> is enabled.
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
        return severity == Severity.DO_NOT_SHOW ? severity : Severity.SUGGESTION;
      }
    }

    #endregion
  }
}