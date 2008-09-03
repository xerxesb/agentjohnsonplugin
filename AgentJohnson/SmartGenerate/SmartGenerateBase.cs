using System.Collections.Generic;
using System.Security;
using JetBrains.ActionManagement;
using JetBrains.Annotations;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Util;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// Defines the smart generate base class.
  /// </summary>
  public abstract class SmartGenerateBase : ISmartGenerate {
    #region Fields

    List<ISmartGenerateMenuItem> _items;

    #endregion

    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">The get menu items parameters.</param>
    /// <returns>The items.</returns>
    public virtual IEnumerable<ISmartGenerateMenuItem> GetMenuItems(SmartGenerateParameters smartGenerateParameters) {
      _items = new List<ISmartGenerateMenuItem>();

      GetItems(smartGenerateParameters);

      return _items;
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Adds the specified text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="template">The template.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The menu item.</returns>
    [CanBeNull]
    protected ISmartGenerateMenuItem AddMenuItem([NotNull] string text, [NotNull] string template, params string[] parameters) {
      return AddMenuItem(text, template, TextRange.InvalidRange, parameters);
    }

    /// <summary>
    /// Adds the specified text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="template">The template.</param>
    /// <param name="selectionRange">The selection range.</param>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The menu item.</returns>
    [CanBeNull]
    protected ISmartGenerateMenuItem AddMenuItem([NotNull] string text, [NotNull] string template, TextRange selectionRange, params string[] parameters) {
      string expandedTemplate = SmartGenerateManager.Instance.GetTemplate(template);

      if(string.IsNullOrEmpty(expandedTemplate)) {
        return null;
      }

      SmartGenerateMenuItem menuItem = new SmartGenerateMenuItem();

      if(parameters.Length > 0) {
        text = string.Format(text, parameters);

        for(int n = 0; n < parameters.Length; n++) {
          parameters[n] = SecurityElement.Escape(parameters[n]);
        }

        expandedTemplate = string.Format(expandedTemplate, parameters);
      }

      menuItem.Text = text;
      menuItem.Template = expandedTemplate;
      menuItem.SelectionRange = selectionRange;

      AddMenuItem(menuItem);

      return menuItem;
    }

    /// <summary>
    /// Adds the menu item.
    /// </summary>
    /// <param name="menuItem">The menu item.</param>
    protected void AddMenuItem([NotNull] ISmartGenerateMenuItem menuItem) {
      _items.Add(menuItem);
    }

    /// <summary>
    /// Adds the menu separator.
    /// </summary>
    protected void AddMenuSeparator() {
      AddMenuItem(new SmartGenerateMenuSeparator());
    }

    /// <summary>
    /// Gets the smart generate items.
    /// </summary>
    /// <param name="smartGenerateParameters">The get menu items parameters.</param>
    protected abstract void GetItems(SmartGenerateParameters smartGenerateParameters);

    /// <summary>
    /// Determines whether [is after last statement] [the specified element].
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>
    /// 	<c>true</c> if [is after last statement] [the specified element]; otherwise, <c>false</c>.
    /// </returns>
    protected static bool IsAfterLastStatement(IElement element) {
      IBlock block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if(block == null) {
        return false;
      }

      if(block.Statements.Count <= 0) {
        return true;
      }

      IStatement statement = block.Statements[block.Statements.Count - 1];
      DocumentRange range = statement.GetDocumentRange();

      int end = range.TextRange.StartOffset + range.TextRange.Length;
      if(end > element.GetTreeTextRange().StartOffset) {
        return false;
      }

      return true;
    }

    #endregion
  }
}