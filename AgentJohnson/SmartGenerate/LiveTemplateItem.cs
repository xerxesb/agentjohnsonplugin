// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LiveTemplateItem.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The live template item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate
{
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using JetBrains.Util;

  /// <summary>
  /// The live template item.
  /// </summary>
  public class LiveTemplateItem
  {
    #region Constants and Fields

    /// <summary>
    /// The _variables.
    /// </summary>
    private readonly Dictionary<string, string> _variables = new Dictionary<string, string>();

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveTemplateItem"/> class.
    /// </summary>
    public LiveTemplateItem()
    {
      this.Range = TextRange.InvalidRange;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    [CanBeNull]
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    [CanBeNull]
    public string MenuText { get; set; }

    /// <summary>
    /// Gets or sets the range.
    /// </summary>
    /// <value>The range.</value>
    public TextRange Range { get; set; }

    /// <summary>
    /// Gets or sets the name of the template.
    /// </summary>
    /// <value>The name of the template.</value>
    [CanBeNull]
    public string Shortcut { get; set; }

    /// <summary>
    /// Gets the variables.
    /// </summary>
    /// <value>The variables.</value>
    [NotNull]
    public Dictionary<string, string> Variables
    {
      get
      {
        return this._variables;
      }
    }

    #endregion
  }
}