using System;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Editor;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.Shell;
using JetBrains.ReSharper.TextControl;
using JetBrains.Util;

namespace AgentJohnson {
  /// <summary>
  /// Represents the base of a context action.
  /// </summary>
  public abstract class ContextActionBase: IContextAction, IBulbItem {
    #region Fields

    readonly ISolution _solution;
    readonly ITextControl _textControl;

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the <see cref="ContextActionBase"/> class.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    public ContextActionBase(ISolution solution, ITextControl textControl) {
      _solution = solution;
      _textControl = textControl;
    }

    #endregion

    #region Public properties

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
    /// Gets the text control.
    /// </summary>
    /// <value>The text control.</value>
    public ITextControl TextControl {
      get {
        return _textControl;
      }
    }

    #endregion

    #region Protected methods

    /// <summary>
    /// Executes this instance.
    /// </summary>
    /// <param name="element">The element.</param>
    protected abstract void Execute(IElement element);

    /// <summary>
    /// Gets the element as the caret position.
    /// </summary>
    /// <returns>The element.</returns>
    protected IElement GetElementAtCaret() {
      IProjectFile projectFile = DocumentManager.GetInstance(_solution).GetProjectFile(_textControl.Document);
      if(projectFile == null) {
        return null;
      }

      PsiManager psiManager = PsiManager.GetInstance(_solution);
      if(psiManager == null){
        return null;
      }

      ICSharpFile file = psiManager.GetPsiFile(projectFile) as ICSharpFile;
      if(file == null) {
        return null;
      }

      return file.FindTokenAt(_textControl.CaretModel.Offset);
    }

    /// <summary>
    /// Modifies the specified handler.
    /// </summary>
    /// <param name="handler">The handler.</param>
    protected void Modify(TransactionHandler handler) {
      PsiManager psiManager = PsiManager.GetInstance(Solution);
      if(psiManager == null){
        return;
      }

      using(ModificationCookie cookie = TextControl.Document.EnsureWritable()) {
        if(cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS) {
          return;
        }

        using(CommandCookie.Create(string.Format("Context Action {0}", GetText()))){
          psiManager.DoTransaction(handler);
        }
      }
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <returns>The items.</returns>
    protected virtual IBulbItem[] GetItems() {
      return new IBulbItem[] { this };
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <returns>The text.</returns>
    protected abstract string GetText();

    /// <summary>
    /// Determines whether this instance is available.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>
    /// 	<c>true</c> if this instance is available; otherwise, <c>false</c>.
    /// </returns>
    protected abstract bool IsAvailable(IElement element);

    #endregion

    #region IBulbAction Members

    /// <summary>
    /// Check if this action is available at the constructed context.
    /// Actions could store precalculated info in <paramref name="cache" /> to share it between different actions
    /// </summary>
    bool IBulbAction.IsAvailable(IUserDataHolder cache) {
      Shell.Instance.AssertReadAccessAllowed();

      IElement element = GetElementAtCaret();
      if(element == null) {
        return false;
      }

      return IsAvailable(element);
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>The items.</value>
    IBulbItem[] IBulbAction.Items {
      get {
        return GetItems();
      }
    }

    #endregion

    #region IBulbItem Members

    /// <summary>
    /// Executes the specified solution.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="textControl">The text control.</param>
    void IBulbItem.Execute(ISolution solution, ITextControl textControl) {
      if(_solution != solution || _textControl != textControl) {
        throw new InvalidOperationException();
      }

      Shell.Instance.AssertReadAccessAllowed();

      IElement element = GetElementAtCaret();
      if(element == null) {
        return;
      }

      Execute(element);
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    string IBulbItem.Text {
      get {
        return GetText();
      }
    }

    #endregion
  }
}