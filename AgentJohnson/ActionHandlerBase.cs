// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionHandlerBase.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Represents a ActionHandlerBase.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson
{
  using JetBrains.ActionManagement;
  using JetBrains.ProjectModel;
  using JetBrains.ReSharper.Psi.Services;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// Represents a ActionHandlerBase.
  /// </summary>
  public abstract class ActionHandlerBase : IActionHandler
  {
    #region Implemented Interfaces

    #region IActionHandler

    /// <summary>
    /// Executes action. Called after Update, that set <c>ActionPresentation.Enabled</c> to <c>true</c>.
    /// </summary>
    /// <param name="context">
    /// The data context.
    /// </param>
    /// <param name="nextExecute">
    /// delegate to call
    /// </param>
    void IActionHandler.Execute(IDataContext context, DelegateExecute nextExecute)
    {
      var solution = context.GetData(JetBrains.IDE.DataConstants.SOLUTION);
      if (solution == null)
      {
        return;
      }

      this.Execute(solution, context);
    }

    /// <summary>
    /// Updates action visual presentation. If presentation.Enabled is set to <c>false</c>, Execute
    /// will not be called.
    /// </summary>
    /// <param name="context">
    /// The data context.
    /// </param>
    /// <param name="presentation">
    /// presentation to update
    /// </param>
    /// <param name="nextUpdate">
    /// delegate to call
    /// </param>
    /// <returns>
    /// The i action handler. update.
    /// </returns>
    bool IActionHandler.Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
      return this.Update(context);
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Gets the element at caret.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// The element at caret.
    /// </returns>
    protected static IElement GetElementAtCaret(IDataContext context)
    {
      var solution = context.GetData(JetBrains.IDE.DataConstants.SOLUTION);
      if (solution == null)
      {
        return null;
      }

      var textControl = context.GetData(JetBrains.IDE.DataConstants.TEXT_CONTROL);
      if (textControl == null)
      {
        return null;
      }

      return TextControlToPsi.GetElementFromCaretPosition<IElement>(solution, textControl);
    }

    /// <summary>
    /// Executes action. Called after Update, that set <c>ActionPresentation.Enabled</c> to <c>true</c>.
    /// </summary>
    /// <param name="solution">
    /// The solution.
    /// </param>
    /// <param name="context">
    /// The context.
    /// </param>
    protected abstract void Execute(ISolution solution, IDataContext context);

    /// <summary>
    /// Updates the specified context.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// The update.
    /// </returns>
    protected virtual bool Update(IDataContext context)
    {
      return context.CheckAllNotNull(JetBrains.IDE.DataConstants.SOLUTION);
    }

    #endregion
  }
}