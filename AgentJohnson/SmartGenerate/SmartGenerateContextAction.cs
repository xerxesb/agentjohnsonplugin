namespace AgentJohnson.SmartGenerate
{
  using System;
  using System.Collections.Generic;
  using System.Drawing;
  using System.Reflection;
  using System.Windows.Forms;
  using System.Xml;
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
  using JetBrains.UI.PopupWindowManager;
  using JetBrains.Util;
  using JetBrains.Util.Special;
  using Scopes;

  /// <summary>
  /// Handles Smart Generation, see <c>Actions.xml</c>
  /// </summary>
  public class SmartGenerateContextAction : IActionHandler
  {
    #region Fields

    private static int scopeIndex;
    private IDataContext dataContext;

    #endregion

    #region Public methods

    ///<summary>
    /// Executes action. Called after Update, that set <see cref="ActionPresentation"/>.Enabled to true.
    ///</summary>
    ///<param name="context"><c>DataContext</c></param>
    ///<param name="nextExecute">delegate to call</param>
    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
      if (!AtLeadInWhitespace(context))
      {
        nextExecute();
        return;
      }

      Execute(context);
    }

    ///<summary>
    /// Updates action visual presentation. If presentation.Enabled is set to false, Execute
    /// will not be called.
    ///</summary>
    ///<param name="context"><c>DataContext</c></param>
    ///<param name="presentation">presentation to update</param>
    ///<param name="nextUpdate">delegate to call</param>
    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
      if (!AtLeadInWhitespace(context))
      {
        return nextUpdate();
      }

      return context.CheckAllNotNull(JetBrains.IDE.DataConstants.SOLUTION);
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Resets the index.
    /// </summary>
    protected virtual void ResetIndex()
    {
      scopeIndex = 0;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Ats the lead in whitespace.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>Returns the boolean.</returns>
    private static bool AtLeadInWhitespace(IDataContext context)
    {
      ITextControl textControl = context.GetData(JetBrains.IDE.DataConstants.TEXT_CONTROL);
      if (textControl == null)
      {
        return false;
      }
      int caretLine = textControl.Document.GetCoordsByOffset(textControl.CaretModel.Offset).Line;
      string line = textControl.Document.GetLine(caretLine);
      string prefix = line.Substring(0, textControl.CaretModel.Offset - textControl.Document.GetLineStartOffset(caretLine));
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
    /// <returns>The element.</returns>
    private static bool GetElementAtCaret(ISolution solution, ITextControl textControl, out IElement element)
    {
      element = null;

      IProjectFile projectFile = DocumentManager.GetInstance(solution).GetProjectFile(textControl.Document);
      if (projectFile == null)
      {
        return false;
      }

      PsiManager psiManager = PsiManager.GetInstance(solution);
      if (psiManager == null)
      {
        return false;
      }

      psiManager.CommitAllDocuments();

      ICSharpFile file = psiManager.GetPsiFile(projectFile) as ICSharpFile;
      if (file == null)
      {
        return false;
      }

      element = file.FindTokenAt(textControl.CaretModel.Offset);

      return true;
    }

    /// <summary>
    /// Adds the menu item.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="item">The item.</param>
    private void AddItem(List<SimpleMenuItem> items, ISmartGenerateAction item)
    {
      string text = item.Text;

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
          Text = text,
          Style = MenuItemStyle.Enabled,
          Tag = item
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
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void CreateLiveTemplates_OnClicked(object sender, EventArgs e)
    {
      SimpleMenuItem simpleMenuItem = sender as SimpleMenuItem;
      if (simpleMenuItem == null)
      {
        return;
      }

      List<LiveTemplateItem> liveTemplates = simpleMenuItem.Tag as List<LiveTemplateItem>;
      if (liveTemplates == null)
      {
        return;
      }

      List<SimpleMenuItem> items = new List<SimpleMenuItem>();

      foreach (LiveTemplateItem template in liveTemplates)
      {
        SimpleMenuItem item = new SimpleMenuItem
        {
          Style = MenuItemStyle.Enabled,
          Text = (template.MenuText ?? string.Empty),
          Tag = template
        };

        item.Clicked += LiveTemplateManager.AddLiveTemplate;

        items.Add(item);
      }

      JetPopupMenu menu = new JetPopupMenu();

      IPopupWindowContext popupWindowContext = this.dataContext.GetData(DataConstants.POPUP_WINDOW_CONTEXT);
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
    /// Executes action. Called after Update, that set <c>ActionPresentation</c>.Enabled to true.
    /// </summary>
    /// <param name="dataContext">The context.</param>
    private void Execute(IDataContext dataContext)
    {
      this.dataContext = dataContext;

      ISolution solution = dataContext.GetData(JetBrains.IDE.DataConstants.SOLUTION);
      ITextControl textControl = dataContext.GetData(JetBrains.IDE.DataConstants.TEXT_CONTROL);

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

      List<SimpleMenuItem> items = new List<SimpleMenuItem>();

      TextRange range = TextRange.InvalidRange;

      List<ScopeEntry> scope = Scope.Populate(element);
      if (scopeIndex >= scope.Count)
      {
        scopeIndex = scope.Count - 1;
      }
      if (scopeIndex < 0)
      {
        scopeIndex = 0;
      }

      foreach (ISmartGenerateHandler handler in SmartGenerateManager.Instance.GetHandlers())
      {
        IEnumerable<ISmartGenerateAction> list = handler.GetMenuItems(new SmartGenerateParameters
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

        foreach (ISmartGenerateAction smartGenerateItem in list)
        {
          this.AddItem(items, smartGenerateItem);

          if (!range.IsValid())
          {
            range = smartGenerateItem.SelectionRange;
          }
        }
      }

      List<LiveTemplateItem> liveTemplates = LiveTemplateManager.Instance.GetLiveTemplates(new SmartGenerateParameters
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

        SimpleMenuItem item = new SimpleMenuItem
        {
          Text = "Create live template",
          Style = MenuItemStyle.Enabled,
          Tag = liveTemplates
        };

        item.Clicked += this.CreateLiveTemplates_OnClicked;

        items.Add(item);
      }

      ShowPopupMenu(dataContext, items);
    }

    /// <summary>
    /// Shows the popup menu.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="items">The items.</param>
    private static void ShowPopupMenu(IDataContext context, List<SimpleMenuItem> items)
    {
      JetPopupMenu menu = new JetPopupMenu();

      IPopupWindowContext popupWindowContext = context.GetData(DataConstants.POPUP_WINDOW_CONTEXT);
      if (popupWindowContext != null)
      {
        menu.Layouter = popupWindowContext.CreateLayouter();
      }

      menu.Caption.Value = WindowlessControl.Create("Smart Generate");
      menu.SetItems(items.ToArray());
      menu.KeyboardAcceleration.SetValue(KeyboardAccelerationFlags.Mnemonics);

      Bitmap right = ImageLoader.GetImage("AgentJohnson.Resources.LeftArrow.png", Assembly.GetExecutingAssembly());
      Bitmap left = ImageLoader.GetImage("AgentJohnson.Resources.RightArrow.png", Assembly.GetExecutingAssembly());

      menu.ToolbarButtons.Add(new ToolbarItemInfo(new PresentableItem(right), "Previous in scope", Keys.Left, false, delegate
      {
        scopeIndex++;
        IUpdatableAction action = ActionManager.Instance.GetAction("SmartGenerate2");
        ActionManager.Instance.ExecuteActionIfAvailable(action as IExecutableAction);
      }));

      menu.ToolbarButtons.Add(new ToolbarItemInfo(new PresentableItem(left), "Next in scope", Keys.None, false, delegate
      {
        scopeIndex--;
        IUpdatableAction action = ActionManager.Instance.GetAction("SmartGenerate2");
        ActionManager.Instance.ExecuteActionIfAvailable(action as IExecutableAction);
      }));

      menu.Show();
    }

    /// <summary>
    /// Handles the Clicked event of the item control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void Generate(object sender, EventArgs e)
    {
      SimpleMenuItem simpleMenuItem = sender as SimpleMenuItem;
      if (simpleMenuItem == null)
      {
        return;
      }

      ISmartGenerateAction item = simpleMenuItem.Tag as ISmartGenerateAction;
      if (item == null)
      {
        return;
      }

      ISolution solution = this.dataContext.GetData(JetBrains.IDE.DataConstants.SOLUTION);
      ITextControl textControl = this.dataContext.GetData(JetBrains.IDE.DataConstants.TEXT_CONTROL);
      if (textControl == null)
      {
        return;
      }

      if (item.HandleClick(sender, e))
      {
        return;
      }

      Template template;

      string templateText = item.Template;

      if (templateText.StartsWith("<Template"))
      {
        XmlDocument doc = new XmlDocument();

        doc.LoadXml(templateText);

        template = Template.CreateFromXml(doc.DocumentElement);
      }
      else
      {
        template = new Template(string.Empty, string.Empty, templateText, true, true, true);
      }

      TextRange range = item.SelectionRange;

      if (range.IsValid())
      {
        textControl.SelectionModel.SetRange(range);
      }

      Shell.Instance.Invocator.ReentrancyGuard.ExecuteOrQueue("Create Live Template", delegate
      {
        using (ReadLockCookie.Create())
        {
          using (CommandCookie.Create("Context Action Create Live Template"))
          {
            LiveTemplatesManager.CreateHotspotSessionFromTemplate(template, solution, textControl).Execute(null);
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
    #region Protected methods

    /// <summary>
    /// Resets the index.
    /// </summary>
    protected override void ResetIndex()
    {
    }

    #endregion
  }
}
