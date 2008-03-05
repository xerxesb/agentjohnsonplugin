using System.Drawing;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Editor;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson {

  /// <summary>
  /// Base suggestion for Agent Johnson suggestions.
  /// </summary>
  public abstract class SuggestionBase : IHighlighting {
    #region Fields

    readonly IElement _element;
    readonly DocumentRange _range;
    readonly string _suggestionName;
    readonly string _toolTip;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SuggestionBase"/> class.
    /// </summary>
    /// <param name="suggestionName">Name of the suggestion.</param>
    /// <param name="element">The element.</param>
    /// <param name="highlightingRange">The highlighting range.</param>
    /// <param name="toolTip">The tool tip.</param>
    public SuggestionBase(string suggestionName, IElement element, DocumentRange highlightingRange, string toolTip) {
      _range = highlightingRange;
      _toolTip = toolTip;
      _element = element;
      _suggestionName = suggestionName;
    }

    #endregion

    #region Public properties

    /// <summary>
    /// Gets the element.
    /// </summary>
    /// <value>The element.</value>
    public IElement Element {
      get {
        return _element;
      }
    }

    /// <summary>
    /// Gets the navigation offset.
    /// </summary>
    /// <value>The navigation offset.</value>
    public int NavigationOffset {
      get {
        return 0;
      }
    }

    /// <summary>
    /// Gets the range.
    /// </summary>
    /// <value>The range.</value>
    public virtual DocumentRange Range {
      get {
        return _range;
      }
    }

    #endregion

    #region IHighlighting Members

    /// <summary>
    /// Attribute of this highlighting in the markup model
    /// </summary>
    /// <value></value>
    public virtual string AttributeId {
      get {
        return HighlightingAttributeIds.GetDefaultAttribute(Severity);
      }
    }

    /// <summary>
    /// Color on gutter for this highlighting
    /// NOTE: Will be called only if Severity == INFO
    /// </summary>
    /// <value></value>
    public virtual Color ColorOnStripe {
      get {
        return Color.Empty;
      }
    }

    /// <summary>
    /// Identifies if the tooltip message should be shown in the status bar when the cursor is over the highlighting
    /// </summary>
    /// <value></value>
    public bool ShowToolTipInStatusBar {
      get {
        return true;
      }
    }

    /// <summary>
    /// Get the overlap resolving policy for this highlighting
    /// </summary>
    /// <value></value>
    public OverlapResolvePolicy OverlapResolvePolicy {
      get {
        return OverlapResolvePolicy.WARNING;
      }
    }

    /// <summary>
    /// Get the severity of this highlighting
    /// </summary>
    /// <value></value>
    public virtual Severity Severity {
      get {
        return HighlightingSettingsManager.Instance.Settings.GetSeverity(_suggestionName);
      }
    }

    /// <summary>
    /// Message for this highlighting to show in tooltip and in status bar (if <see cref="P:JetBrains.ReSharper.Daemon.IHighlighting.ShowToolTipInStatusBar"/> is <c>true</c>)
    /// To override the default mechanism of tooltip, mark the implementation class with
    /// <see cref="T:JetBrains.ReSharper.Daemon.DaemonTooltipProviderAttribute"/> attribute, and then this property will not be called
    /// </summary>
    /// <value></value>
    public virtual string ToolTip {
      get {
        return _toolTip;
      }
    }

    /// <summary>
    /// Message for this highlighting to show in tooltip and in status bar (if <see cref="P:JetBrains.ReSharper.Daemon.IHighlighting.ShowToolTipInStatusBar"/> is <c>true</c>)
    /// </summary>
    /// <value></value>
    public string ErrorStripeToolTip {
      get {
        return ToolTip;
      }
    }

    /// <summary>
    /// Specifies the offset from the <c>Range.StartOffset</c> to set the cursor to when navigating
    /// to this highlighting. Usually returns <c>0</c>
    /// </summary>
    /// <value></value>
    public int NavigationOffsetPatch {
      get {
        return 0;
      }
    }

    #endregion
  }
}