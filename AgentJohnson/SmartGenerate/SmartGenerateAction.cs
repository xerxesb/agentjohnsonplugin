using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using AgentJohnson.SmartGenerate.Scopes;
using JetBrains.ActionManagement;
using JetBrains.CommonControls;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.CodeInsight.Services.Util;
using JetBrains.ReSharper.LiveTemplates.Execution;
using JetBrains.ReSharper.LiveTemplates.Templates;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;
using JetBrains.UI;
using JetBrains.UI.PopupMenu;
using JetBrains.UI.PopupWindowManager;
using JetBrains.Util;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// Handles Smart Generation, see <c>Actions.xml</c>
  /// </summary>
  public class SmartGenerateAction : IActionHandler {
    #region Fields

    protected static int _scopeIndex;
    IDataContext _context;
    ISolution _solution;
    ITextControl _textControl;

    #endregion

    #region Public methods

    ///<summary>
    /// Executes action. Called after Update, that set <see cref="ActionPresentation"/>.Enabled to true.
    ///</summary>
    ///<param name="context"><c>DataContext</c></param>
    ///<param name="nextExecute">delegate to call</param>
    public void Execute(IDataContext context, DelegateExecute nextExecute) {
      ISolution solution = context.GetData(JetBrains.IDE.DataConstants.SOLUTION);
      if(solution == null) {
        return;
      }

      Execute(solution, context);
    }

    ///<summary>
    /// Updates action visual presentation. If presentation.Enabled is set to false, Execute
    /// will not be called.
    ///</summary>
    ///<param name="context"><c>DataContext</c></param>
    ///<param name="presentation">presentation to update</param>
    ///<param name="nextUpdate">delegate to call</param>
    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate) {
      return context.CheckAllNotNull(JetBrains.IDE.DataConstants.SOLUTION);
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes action. Called after Update, that set <c>ActionPresentation</c>.Enabled to true.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    protected void Execute(ISolution solution, IDataContext context) {
      _solution = solution;
      _context = context;
      _textControl = context.GetData(JetBrains.IDE.DataConstants.TEXT_CONTROL);

      ResetIndex();

      IElement element;

      if(!GetElementAtCaret(_solution, _textControl, out element)) {
        return;
      }

      if(element == null) {
        return;
      }

      List<SimpleMenuItem> items = new List<SimpleMenuItem>();

      TextRange range = TextRange.InvalidRange;

      List<ScopeEntry> scope = Scope.Populate(element);
      if(_scopeIndex >= scope.Count) {
        _scopeIndex = scope.Count - 1;
      }
      if(_scopeIndex < 0) {
        _scopeIndex = 0;
      }

      foreach(ISmartGenerate handler in SmartGenerateManager.Instance.GetHandlers()) {
        IEnumerable<ISmartGenerateMenuItem> list = handler.GetMenuItems(new SmartGenerateParameters {
          Solution = solution,
          Context = context,
          Element = element,
          Scope = scope,
          ScopeIndex = _scopeIndex
        });

        if(list == null) {
          continue;
        }

        foreach(ISmartGenerateMenuItem smartGenerateItem in list) {
          AddItem(items, smartGenerateItem);

          if(!range.IsValid) {
            range = smartGenerateItem.SelectionRange;
          }
        }
      }

      List<LiveTemplateItem> liveTemplates = LiveTemplateManager.Instance.GetLiveTemplates(new SmartGenerateParameters {
        Solution = solution,
        Context = context,
        Element = element,
        Scope = scope,
        ScopeIndex = _scopeIndex
      });

      if(liveTemplates.Count > 0) {
        items.Add(new SimpleMenuItem {
          Style = MenuItemStyle.Separator
        });

        SimpleMenuItem item = new SimpleMenuItem {
          Text = "Create live template",
          Style = MenuItemStyle.Enabled,
          Tag = liveTemplates
        };

        item.Clicked += CreateLiveTemplates_OnClicked;

        items.Add(item);
      }

      JetPopupMenu menu = new JetPopupMenu();

      IPopupWindowContext popupWindowContext = context.GetData(JetBrains.UI.DataConstants.POPUP_WINDOW_CONTEXT);
      if(popupWindowContext != null) {
        menu.Layouter = popupWindowContext.CreateLayouter();
      }

      menu.Caption.Value = WindowlessControl.Create("Smart Generate");
      menu.SetItems(items);
      menu.KeyboardAcceleration.SetValue(KeyboardAccelerationFlags.Mnemonics);

      Bitmap right = ImageLoader.GetImage("AgentJohnson.Resources.LeftArrow.png", Assembly.GetExecutingAssembly());
      Bitmap left = ImageLoader.GetImage("AgentJohnson.Resources.RightArrow.png", Assembly.GetExecutingAssembly());

      menu.ToolbarButtons.Add(new ToolbarItemInfo(new PresentableItem(right), "Previous in scope", Keys.Left, false, delegate {
        _scopeIndex++;
        IUpdatableAction action = ActionManager.Instance.GetAction("SmartGenerate2");
        ActionManager.Instance.ExecuteAction(action as IExecutableAction);
      }));

      menu.ToolbarButtons.Add(new ToolbarItemInfo(new PresentableItem(left), "Next in scope", Keys.None, false, delegate {
        _scopeIndex--;
        IUpdatableAction action = ActionManager.Instance.GetAction("SmartGenerate2");
        ActionManager.Instance.ExecuteAction(action as IExecutableAction);
      }));

      menu.Show();
    }

    /// <summary>
    /// Gets the element as the caret position.
    /// </summary>
    /// <returns>The element.</returns>
    protected bool GetElementAtCaret(ISolution solution, ITextControl textControl, out IElement element) {
      element = null;

      IProjectFile projectFile = DocumentManager.GetInstance(solution).GetProjectFile(textControl.Document);
      if(projectFile == null) {
        return false;
      }

      PsiManager psiManager = PsiManager.GetInstance(solution);
      if(psiManager == null) {
        return false;
      }

      psiManager.CommitAndWaitForCaches(this);

      ICSharpFile file = psiManager.GetPsiFile(projectFile) as ICSharpFile;
      if(file == null) {
        return false;
      }

      element = file.FindTokenAt(textControl.CaretModel.Offset);

      return true;
    }

    /// <summary>
    /// Resets the index.
    /// </summary>
    protected virtual void ResetIndex() {
      _scopeIndex = 0;
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Adds the menu item.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="item">The item.</param>
    void AddItem(List<SimpleMenuItem> items, ISmartGenerateMenuItem item) {
      string text = item.Text;

      SimpleMenuItem simpleMenuItem;

      if(text == "-") {
        simpleMenuItem = new SimpleMenuItem {
          Style = MenuItemStyle.Separator
        };
      }
      else {
        simpleMenuItem = new SimpleMenuItem {
          Text = text,
          Style = MenuItemStyle.Enabled,
          Tag = item
        };

        if(items.Count < 9) {
          simpleMenuItem.Mnemonic = (items.Count + 1).ToString();
          simpleMenuItem.ShortcutText = simpleMenuItem.Mnemonic;
        }

        simpleMenuItem.Clicked += Generate;
      }

      items.Add(simpleMenuItem);
    }

    /// <summary>
    /// Handles the OnClicked event of the CreateLiveTemplates control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void CreateLiveTemplates_OnClicked(object sender, EventArgs e) {
      SimpleMenuItem simpleMenuItem = sender as SimpleMenuItem;
      if(simpleMenuItem == null) {
        return;
      }

      List<LiveTemplateItem> liveTemplates = simpleMenuItem.Tag as List<LiveTemplateItem>;
      if(liveTemplates == null) {
        return;
      }

      List<SimpleMenuItem> items = new List<SimpleMenuItem>();

      foreach(LiveTemplateItem template in liveTemplates) {
        SimpleMenuItem item = new SimpleMenuItem {
          Style = MenuItemStyle.Enabled,
          Text = (template.MenuText ?? string.Empty),
          Tag = template
        };

        item.Clicked += LiveTemplateManager.AddLiveTemplate;

        items.Add(item);
      }

      JetPopupMenu menu = new JetPopupMenu();

      IPopupWindowContext popupWindowContext = _context.GetData(DataConstants.POPUP_WINDOW_CONTEXT);
      if(popupWindowContext != null) {
        menu.Layouter = popupWindowContext.CreateLayouter();
      }

      menu.Caption.Value = WindowlessControl.Create("Create live template");
      menu.SetItems(items);
      menu.KeyboardAcceleration.SetValue(KeyboardAccelerationFlags.Mnemonics);

      menu.Show();
    }

    /// <summary>
    /// Handles the Clicked event of the item control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    void Generate(object sender, EventArgs e) {
      SimpleMenuItem simpleMenuItem = sender as SimpleMenuItem;
      if(simpleMenuItem == null) {
        return;
      }

      ISmartGenerateMenuItem item = simpleMenuItem.Tag as ISmartGenerateMenuItem;
      if(item == null) {
        return;
      }

      if(item.HandleClick(sender, e)) {
        return;
      }

      Template template;

      string templateText = item.Template;

      if(templateText.StartsWith("<Template")) {
        XmlDocument doc = new XmlDocument();

        doc.LoadXml(templateText);

        template = Template.CreateFromXml(doc.DocumentElement);
      }
      else {
        template = new Template(string.Empty, string.Empty, templateText, true, true);
      }

      TextRange range = item.SelectionRange;

      if(range.IsValid) {
        _textControl.SelectionModel.SetRange(range);
      }

      LiveTemplatesController.Instance.ExecuteTemplate(_solution, template, _textControl);
    }

    #endregion
  }

  /// <summary>
  /// Defines the smart generate action1 class.
  /// </summary>
  [ActionHandler("SmartGenerate")]
  public class SmartGenerateAction1 : SmartGenerateAction {
  }

  /// <summary>
  /// Defines the smart generate action2 class.
  /// </summary>
  [ActionHandler("SmartGenerate2")]
  public class SmartGenerateAction2 : SmartGenerateAction {
    #region Protected methods

    /// <summary>
    /// Resets the index.
    /// </summary>
    protected override void ResetIndex() {
    }

    #endregion
  }
}