// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SmartGenerateContextAction.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Handles Smart Generation, see &lt;c&gt;Actions.xml&lt;/c&gt;
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.SmartGenerate
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;
  using System.Windows.Forms;
  using System.Xml;
  using Scopes;
  using JetBrains.ActionManagement;
  using JetBrains.Application;
  using JetBrains.CommonControls;
  using JetBrains.DocumentModel;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Feature.Services.LiveTemplates.LiveTemplates;
  using JetBrains.ReSharper.LiveTemplates.Templates;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.TextControl;
  using JetBrains.UI;
  using JetBrains.UI.PopupMenu;
  using JetBrains.Util;
  using JetBrains.Util.Special;

  /// <summary>
  /// Handles Smart Generation, see <c>Actions.xml</c>
  /// </summary>
  public class SmartGenerateContextAction : IActionHandler
  {
    #region Constants and Fields

    /// <summary>
    /// The scope index.
    /// </summary>
    private static int scopeIndex;

    /// <summary>
    /// The data context.
    /// </summary>
    private IDataContext dataContext;

    #endregion

    #region Implemented Interfaces

    #region IActionHandler

    /// <summary>
    /// Executes action. Called after Update, that set <see cref="ActionPresentation"/>. Enabled to <c>true</c>.
    /// </summary>
    /// <param name="context">The data context.</param>
    /// <param name="nextExecute">Delegate to call.</param>
    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
      if (!AtLeadInWhitespace(context))
      {
        nextExecute();
        return;
      }

      this.Execute(context);
    }

    /// <summary>
    /// Updates action visual presentation. If presentation.Enabled is set to <c>false</c>, Execute
    /// will not be called.
    /// </summary>
    /// <param name="context">The data context.</param>
    /// <param name="presentation">presentation to update</param>
    /// <param name="nextUpdate">delegate to call</param>
    /// <returns><c>true</c>, if successful.</returns>
    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
      if (!AtLeadInWhitespace(context))
      {
        return nextUpdate();
      }

      return context.CheckAllNotNull(JetBrains.IDE.DataConstants.SOLUTION);
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Resets the index.
    /// </summary>
    protected virtual void ResetIndex()
    {
      scopeIndex = 0;
    }

    /// <summary>
    /// Ats the lead in whitespace.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// Returns the boolean.
    /// </returns>
    private static bool AtLeadInWhitespace(IDataContext context)
    {
      var textControl = context.GetData(JetBrains.IDE.DataConstants.TEXT_CONTROL);
      if (textControl == null)
      {
        return false;
      }

      var caretLine = textControl.Document.GetCoordsByOffset(textControl.CaretModel.Offset).Line;
      var line = textControl.Document.GetLine(caretLine);
      var prefix = line.Substring(
        0, textControl.CaretModel.Offset - textControl.Document.GetLineStartOffset(caretLine));
      if (prefix.TrimStart(new[]
      {
        ' ', '\t'
      }).Length != 0)
      {
        return false;
      }

      if ((prefix.Length != 0) && (line.Length != prefix.Length))
      {
        return line[prefix.Length].IsAnyOf(new[]
        {
          ' ', '\t'
        });
      }

      return true;
    }

    /// <summary>
    /// Gets the element as the caret position.
    /// </summary>
    /// <param name="solution">
    /// The solution.
    /// </param>
    /// <param name="textControl">
    /// The text Control.
    /// </param>
    /// <param name="element">
    /// The element.
    /// </param>
    /// <returns>
    /// <c>true</c>, if successful.
    /// </returns>
    private static bool GetElementAtCaret(ISolution solution, ITextControl textControl, out IElement element)
    {
      element = null;

      var projectFile = DocumentManager.GetInstance(solution).GetProjectFile(textControl.Document);
      if (projectFile == null)
      {
        return false;
      }

      var psiManager = PsiManager.GetInstance(solution);
      if (psiManager == null)
      {
        return false;
      }

      psiManager.CommitAllDocuments();

      var file = psiManager.GetPsiFile(projectFile) as ICSharpFile;
      if (file == null)
      {
        return false;
      }

      element = file.FindTokenAt(textControl.CaretModel.Offset);

      return true;
    }

    /// <summary>
    /// Shows the popup menu.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="items">
    /// The items.
    /// </param>
    private static void ShowPopupMenu(IDataContext context, List<SimpleMenuItem> items)
    {
      var menu = new JetPopupMenu();

      var popupWindowContext = context.GetData(DataConstants.POPUP_WINDOW_CONTEXT);
      if (popupWindowContext != null)
      {
        menu.Layouter = popupWindowContext.CreateLayouter();
      }

      menu.Caption.Value = WindowlessControl.Create("Smart Generate [Agent Johnson]");
      menu.SetItems(items.ToArray());
      menu.KeyboardAcceleration.SetValue(KeyboardAccelerationFlags.Mnemonics);

      var right = ImageLoader.GetImage("AgentJohnson.Resources.LeftArrow.png", Assembly.GetExecutingAssembly());
      var left = ImageLoader.GetImage("AgentJohnson.Resources.RightArrow.png", Assembly.GetExecutingAssembly());

      menu.ToolbarButtons.Add(
        new ToolbarItemInfo(
          new PresentableItem(right),
          "Previous in scope",
          Keys.Left,
          false,
          delegate
          {
            Shell.Instance.Invocator.ReentrancyGuard.ExecuteOrQueue(
              "SmartGenerate2", 
              delegate
            {
              scopeIndex++;
              var action = ActionManager.Instance.GetAction("SmartGenerate2") as IExecutableAction;
              if (action != null)
              {
                ActionManager.Instance.ExecuteActionIfAvailable(action);
              }
            });
          }));

      menu.ToolbarButtons.Add(
        new ToolbarItemInfo(
          new PresentableItem(left),
          "Next in scope",
          Keys.None,
          false,
          delegate
          {
            Shell.Instance.Invocator.ReentrancyGuard.ExecuteOrQueue(
              "SmartGenerate2",
              delegate
              {
                scopeIndex--;
                var action = ActionManager.Instance.GetAction("SmartGenerate2") as IExecutableAction;
                if (action != null)
                {
                  ActionManager.Instance.ExecuteActionIfAvailable(action);
                }
              });
          }));

      menu.Show();
    }

    /// <summary>
    /// Adds the menu item.
    /// </summary>
    /// <param name="items">The list of items.</param>
    /// <param name="item">The current item.</param>
    private void AddItem(List<SimpleMenuItem> items, ISmartGenerateAction item)
    {
      var text = item.Text;

      SimpleMenuItem simpleMenuItem;

      if (text == "-")
      {
        simpleMenuItem = new SimpleMenuItem
        {
          Style = MenuItemStyle.Separator
        };
      }
      else
      {
        simpleMenuItem = new SimpleMenuItem
        {
          Text = text, Style = MenuItemStyle.Enabled, Tag = item
        };

        if (items.Count < 9)
        {
          simpleMenuItem.Mnemonic = (items.Count + 1).ToString();
          simpleMenuItem.ShortcutText = simpleMenuItem.Mnemonic;
        }

        simpleMenuItem.Clicked += this.Generate;
      }

      items.Add(simpleMenuItem);
    }

    /// <summary>
    /// Handles the OnClicked event of the CreateLiveTemplates control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data.
    /// </param>
    private void CreateLiveTemplates_OnClicked(object sender, EventArgs e)
    {
      var simpleMenuItem = sender as SimpleMenuItem;
      if (simpleMenuItem == null)
      {
        return;
      }

      var liveTemplates = simpleMenuItem.Tag as List<LiveTemplateItem>;
      if (liveTemplates == null)
      {
        return;
      }

      var items = new List<SimpleMenuItem>();

      foreach (var template in liveTemplates)
      {
        var item = new SimpleMenuItem
        {
          Style = MenuItemStyle.Enabled,
          Text = template.MenuText ?? string.Empty,
          Tag = template
        };

        item.Clicked += LiveTemplateManager.AddLiveTemplate;

        items.Add(item);
      }

      var menu = new JetPopupMenu();

      var popupWindowContext = this.dataContext.GetData(DataConstants.POPUP_WINDOW_CONTEXT);
      if (popupWindowContext != null)
      {
        menu.Layouter = popupWindowContext.CreateLayouter();
      }

      menu.Caption.Value = WindowlessControl.Create("Create live template");
      menu.SetItems(items.ToArray());
      menu.KeyboardAcceleration.SetValue(KeyboardAccelerationFlags.Mnemonics);

      menu.Show();
    }

    /// <summary>
    /// Executes action. Called after Update, that set <c>ActionPresentation</c>.Enabled to <c>true</c>.
    /// </summary>
    /// <param name="dataContext">The context.</param>
    private void Execute(IDataContext dataContext)
    {
      this.dataContext = dataContext;

      var solution = dataContext.GetData(JetBrains.IDE.DataConstants.SOLUTION);
      var textControl = dataContext.GetData(JetBrains.IDE.DataConstants.TEXT_CONTROL);

      this.ResetIndex();

      IElement element;
      if (!GetElementAtCaret(solution, textControl, out element))
      {
        return;
      }

      if (element == null)
      {
        return;
      }

      var items = new List<SimpleMenuItem>();

      var range = TextRange.InvalidRange;

      var scope = Scope.Populate(element);
      if (scopeIndex >= scope.Count)
      {
        scopeIndex = scope.Count - 1;
      }

      if (scopeIndex < 0)
      {
        scopeIndex = 0;
      }

      foreach (var handler in SmartGenerateManager.Instance.GetHandlers())
      {
        var list =
          handler.GetMenuItems(
            new SmartGenerateParameters
            {
              Solution = solution,
              TextControl = textControl,
              Context = dataContext,
              Element = element,
              Scope = scope,
              ScopeIndex = scopeIndex
            });

        if (list == null)
        {
          continue;
        }

        foreach (var smartGenerateItem in list)
        {
          this.AddItem(items, smartGenerateItem);

          if (!range.IsValid())
          {
            range = smartGenerateItem.SelectionRange;
          }
        }
      }

      var liveTemplates =
        LiveTemplateManager.Instance.GetLiveTemplates(
          new SmartGenerateParameters
          {
            TextControl = textControl,
            Solution = solution,
            Context = dataContext,
            Element = element,
            Scope = scope,
            ScopeIndex = scopeIndex
          });

      if (liveTemplates.Count > 0)
      {
        items.Add(new SimpleMenuItem
        {
          Style = MenuItemStyle.Separator
        });

        var item = new SimpleMenuItem
        {
          Text = "Create live template", Style = MenuItemStyle.Enabled, Tag = liveTemplates
        };

        item.Clicked += this.CreateLiveTemplates_OnClicked;

        items.Add(item);
      }

      ShowPopupMenu(dataContext, items);
    }

    /// <summary>
    /// Handles the Clicked event of the item control.
    /// </summary>
    /// <param name="sender">
    /// The source of the event.
    /// </param>
    /// <param name="e">
    /// The <see cref="System.EventArgs"/> instance containing the event data.
    /// </param>
    private void Generate(object sender, EventArgs e)
    {
      var simpleMenuItem = sender as SimpleMenuItem;
      if (simpleMenuItem == null)
      {
        return;
      }

      var item = simpleMenuItem.Tag as ISmartGenerateAction;
      if (item == null)
      {
        return;
      }

      var solution = this.dataContext.GetData(JetBrains.IDE.DataConstants.SOLUTION);
      var textControl = this.dataContext.GetData(JetBrains.IDE.DataConstants.TEXT_CONTROL);
      if (textControl == null)
      {
        return;
      }

      if (item.HandleClick(sender, e))
      {
        return;
      }

      Template template;

      var templateText = item.Template;

      if (templateText.StartsWith("<Template"))
      {
        var doc = new XmlDocument();

        doc.LoadXml(templateText);

        var documentElement = doc.DocumentElement;
        if (documentElement == null)
        {
          return;
        }

        template = Template.CreateFromXml(documentElement);
      }
      else
      {
        template = new Template(string.Empty, string.Empty, templateText, true, true, true);
      }

      var range = item.SelectionRange;

      if (range.IsValid())
      {
        textControl.SelectionModel.SetRange(range);
      }

      Shell.Instance.Invocator.ReentrancyGuard.ExecuteOrQueue(
        "Create Live Template",
        delegate
        {
          using (ReadLockCookie.Create())
          {
            using (CommandCookie.Create("Context Action Create Live Template"))
            {
              var hotspotSession = LiveTemplatesManager.CreateHotspotSessionFromTemplate(template, solution, textControl);
              if (hotspotSession != null)
              {
                hotspotSession.Execute(null);
              }
            }
          }
        });
    }

    #endregion
  }

  /// <summary>
  /// Defines the smart generate action1 class.
  /// </summary>
  [ActionHandler("SmartGenerate")]
  public class SmartGenerateAction1 : SmartGenerateContextAction
  {
  }

  /// <summary>
  /// Defines the smart generate action2 class.
  /// </summary>
  [ActionHandler("SmartGenerate2")]
  public class SmartGenerateAction2 : SmartGenerateContextAction
  {
    #region Methods

    /// <summary>
    /// Resets the index.
    /// </summary>
    protected override void ResetIndex()
    {
    }

    #endregion
  }
}