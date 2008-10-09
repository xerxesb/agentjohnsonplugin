using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;

namespace AgentJohnson {
  /// <summary>
  /// Represents a ActionHandlerBase.
  /// </summary>
  public abstract class ActionHandlerBase : IActionHandler {
    #region Protected methods

    /// <summary>
    /// Executes action. Called after Update, that set ActionPresentation.Enabled to true.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    protected abstract void Execute(ISolution solution, IDataContext context);

    /// <summary>
    /// Gets the element at caret.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>The element at caret.</returns>
    protected static IElement GetElementAtCaret(IDataContext context) {
      ISolution solution = context.GetData(JetBrains.IDE.DataConstants.SOLUTION);
      if(solution == null) {
        return null;
      }

      ITextControl textControl = context.GetData(JetBrains.IDE.DataConstants.TEXT_CONTROL);
      if(textControl == null) {
        return null;
      }

      return TextControlToPsi.GetElementFromCaretPosition<IElement>(solution, textControl);
    }

    /// <summary>
    /// Updates the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns></returns>
    protected virtual bool Update(IDataContext context) {
      return context.CheckAllNotNull(JetBrains.IDE.DataConstants.SOLUTION);
    }

    #endregion

    #region Private methods

    /// <summary>
    /// Executes action. Called after Update, that set ActionPresentation.Enabled to true.
    /// </summary>
    /// <param name="context">DataContext</param>
    /// <param name="nextExecute">delegate to call</param>
    void IActionHandler.Execute(IDataContext context, DelegateExecute nextExecute) {
      ISolution solution = context.GetData(JetBrains.IDE.DataConstants.SOLUTION);
      if(solution == null) {
        return;
      }

      Execute(solution, context);
    }

    /// <summary>
    /// Updates action visual presentation. If presentation.Enabled is set to false, Execute
    /// will not be called.
    /// </summary>
    /// <param name="context">DataContext</param>
    /// <param name="presentation">presentation to update</param>
    /// <param name="nextUpdate">delegate to call</param>
    bool IActionHandler.Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate) {
      return Update(context);
    }

    #endregion
  }
}