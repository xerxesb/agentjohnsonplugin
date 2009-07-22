// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmartGenerateHandlerBase.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Defines the smart generate base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate
{
  using System.Collections.Generic;
  using System.Security;
  using JetBrains.Annotations;
  using JetBrains.Util;

  /// <summary>
  /// Defines the smart generate base class.
  /// </summary>
  public abstract class SmartGenerateHandlerBase : ISmartGenerateHandler
  {
    #region Constants and Fields

    /// <summary>
    /// The _items.
    /// </summary>
    private List<ISmartGenerateAction> _items;

    #endregion

    #region Implemented Interfaces

    #region ISmartGenerateHandler

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">
    /// The get menu items parameters.
    /// </param>
    /// <returns>
    /// The items.
    /// </returns>
    public virtual IEnumerable<ISmartGenerateAction> GetMenuItems(SmartGenerateParameters smartGenerateParameters)
    {
      this._items = new List<ISmartGenerateAction>();

      this.GetItems(smartGenerateParameters);

      return this._items;
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Adds the specified text.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="template">
    /// The template.
    /// </param>
    /// <param name="parameters">
    /// The parameters.
    /// </param>
    /// <returns>
    /// The menu item.
    /// </returns>
    [CanBeNull]
    protected ISmartGenerateAction AddAction([NotNull] string text, [NotNull] string template, params string[] parameters)
    {
      return this.AddAction(text, template, TextRange.InvalidRange, parameters);
    }

    /// <summary>
    /// Adds the specified text.
    /// </summary>
    /// <param name="text">
    /// The text.
    /// </param>
    /// <param name="template">
    /// The template.
    /// </param>
    /// <param name="selectionRange">
    /// The selection range.
    /// </param>
    /// <param name="parameters">
    /// The parameters.
    /// </param>
    /// <returns>
    /// The menu item.
    /// </returns>
    [CanBeNull]
    protected ISmartGenerateAction AddAction([NotNull] string text, [NotNull] string template, TextRange selectionRange, params string[] parameters)
    {
      var expandedTemplate = SmartGenerateManager.Instance.GetTemplate(template);

      if (string.IsNullOrEmpty(expandedTemplate))
      {
        return null;
      }

      var action = new SmartGenerateAction();

      if (parameters.Length > 0)
      {
        text = string.Format(text, parameters);

        for (var n = 0; n < parameters.Length; n++)
        {
          parameters[n] = SecurityElement.Escape(parameters[n]);
        }

        expandedTemplate = string.Format(expandedTemplate, parameters);
      }

      action.Text = text;
      action.Template = expandedTemplate;
      action.SelectionRange = selectionRange;

      this.AddAction(action);

      return action;
    }

    /// <summary>
    /// Adds the menu item.
    /// </summary>
    /// <param name="action">
    /// The menu item.
    /// </param>
    protected void AddAction([NotNull] ISmartGenerateAction action)
    {
      this._items.Add(action);
    }

    /// <summary>
    /// Adds the menu separator.
    /// </summary>
    protected void AddMenuSeparator()
    {
      this.AddAction(new SmartGenerateMenuSeparator());
    }

    /// <summary>
    /// Gets the smart generate items.
    /// </summary>
    /// <param name="smartGenerateParameters">
    /// The get menu items parameters.
    /// </param>
    protected abstract void GetItems(SmartGenerateParameters smartGenerateParameters);

    #endregion
  }
}