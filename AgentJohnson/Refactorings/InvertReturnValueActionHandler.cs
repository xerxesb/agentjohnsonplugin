// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvertReturnValueActionHandler.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   The invert return value action handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.Refactorings
{
  using JetBrains.ActionManagement;
  using JetBrains.IDE;
  using JetBrains.ProjectModel;

  /// <summary>
  /// The invert return value action handler.
  /// </summary>
  [ActionHandler("AgentJohnson.InvertReturnValue")]
  public class InvertReturnValueActionHandler : ActionHandlerBase
  {
    #region Methods

    /// <summary>
    /// Executes action. Called after Update, that set ActionPresentation.Enabled to true.
    /// </summary>
    /// <param name="solution">
    /// The solution.
    /// </param>
    /// <param name="context">
    /// The context.
    /// </param>
    protected override void Execute(ISolution solution, IDataContext context)
    {
      var textControl = context.GetData(DataConstants.TEXT_CONTROL);
      if (textControl == null)
      {
        return;
      }

      var invertReturnValueRefactoring = new InvertReturnValueRefactoring(solution, textControl);

      invertReturnValueRefactoring.Execute();
    }

    /// <summary>
    /// Updates the specified context.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <returns>
    /// The update.
    /// </returns>
    protected override bool Update(IDataContext context)
    {
      if (!context.CheckAllNotNull(DataConstants.SOLUTION))
      {
        return false;
      }

      var element = GetElementAtCaret(context);
      if (element == null)
      {
        return false;
      }

      return InvertReturnValueRefactoring.IsAvailable(element);
    }

    #endregion
  }
}