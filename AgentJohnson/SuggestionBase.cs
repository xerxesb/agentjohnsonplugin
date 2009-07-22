// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuggestionBase.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Base suggestion for Agent Johnson suggestions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson
{
  using System.Drawing;
  using JetBrains.DocumentModel;
  using JetBrains.ReSharper.Daemon;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Base suggestion for Agent Johnson suggestions.
  /// </summary>
  public abstract class SuggestionBase : IHighlighting
  {
    #region Constants and Fields

    /// <summary>
    /// The _element.
    /// </summary>
    private readonly IElement _element;

    /// <summary>
    /// The _range.
    /// </summary>
    private readonly DocumentRange _range;

    /// <summary>
    /// The _suggestion name.
    /// </summary>
    private readonly string _suggestionName;

    /// <summary>
    /// The _tool tip.
    /// </summary>
    private readonly string _toolTip;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SuggestionBase"/> class.
    /// </summary>
    /// <param name="suggestionName">
    /// Name of the suggestion.
    /// </param>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <param name="highlightingRange">
    /// The highlighting range.
    /// </param>
    /// <param name="toolTip">
    /// The tool tip.
    /// </param>
    public SuggestionBase(string suggestionName, IElement element, DocumentRange highlightingRange, string toolTip)
    {
      this._range = highlightingRange;
      this._toolTip = toolTip;
      this._element = element;
      this._suggestionName = suggestionName;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the attribute id.
    /// </summary>
    /// <value>The attribute id.</value>
    public virtual string AttributeId
    {
      get
      {
        switch (this.Severity)
        {
          case Severity.ERROR:
            return HighlightingAttributeIds.ERROR_ATTRIBUTE;
          case Severity.WARNING:
            return HighlightingAttributeIds.WARNING_ATTRIBUTE;
          case Severity.SUGGESTION:
            return HighlightingAttributeIds.SUGGESTION_ATTRIBUTE;
          case Severity.HINT:
            return HighlightingAttributeIds.HINT_ATTRIBUTE;
          case Severity.INFO:
          case Severity.DO_NOT_SHOW:
            return null;
        }

        return null;
      }
    }

    /// <summary>
    /// Color on gutter for this highlighting
    /// NOTE: Will be called only if Severity == INFO
    /// </summary>
    /// <value></value>
    public virtual Color ColorOnStripe
    {
      get
      {
        return Color.Empty;
      }
    }

    /// <summary>
    /// Gets the element.
    /// </summary>
    /// <value>The element.</value>
    public IElement Element
    {
      get
      {
        return this._element;
      }
    }

    /// <summary>
    /// Message for this highlighting to show in tooltip and in status bar (if <see cref="P:JetBrains.ReSharper.Daemon.HighlgihtingAttributeBase.ShowToolTipInStatusBar"/> is <c>true</c>)
    /// </summary>
    /// <value></value>
    public string ErrorStripeToolTip
    {
      get
      {
        return this.ToolTip;
      }
    }

    /// <summary>
    /// Gets the navigation offset.
    /// </summary>
    /// <value>The navigation offset.</value>
    public int NavigationOffset
    {
      get
      {
        return 0;
      }
    }

    /// <summary>
    /// Specifies the offset from the <c>Range.StartOffset</c> to set the cursor to when navigating
    /// to this highlighting. Usually returns <c>0</c>
    /// </summary>
    /// <value></value>
    public int NavigationOffsetPatch
    {
      get
      {
        return 0;
      }
    }

    /// <summary>
    /// Gets the range.
    /// </summary>
    /// <value>The range.</value>
    public virtual DocumentRange Range
    {
      get
      {
        return this._range;
      }
    }

    /// <summary>
    /// Gets the severity of this highlighting
    /// </summary>
    /// <value>The severity.</value>
    public virtual Severity Severity
    {
      get
      {
        return HighlightingSettingsManager.Instance.Settings.GetSeverity(this._suggestionName);
      }
    }

    /// <summary>
    /// Identifies if the tooltip message should be shown in the status bar when the cursor is over the highlighting
    /// </summary>
    /// <value></value>
    public bool ShowToolTipInStatusBar
    {
      get
      {
        return true;
      }
    }

    /// <summary>
    /// Message for this highlighting to show in tooltip and in status bar (if <see cref="P:JetBrains.ReSharper.Daemon.HighlgihtingAttributeBase.ShowToolTipInStatusBar"/> is <c>true</c>)
    /// To override the default mechanism of tooltip, mark the implementation class with
    /// <see cref="T:JetBrains.ReSharper.Daemon.DaemonTooltipProviderAttribute"/> attribute, and then this property will not be called
    /// </summary>
    /// <value></value>
    public virtual string ToolTip
    {
      get
      {
        return this._toolTip;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IHighlighting

    /// <summary>
    /// Returns true if data (PSI, text ranges) associated with highlighting is valid
    /// </summary>
    /// <returns>
    /// The is valid.
    /// </returns>
    public bool IsValid()
    {
      return true;
    }

    #endregion

    #endregion
  }
}