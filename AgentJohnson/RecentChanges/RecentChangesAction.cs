// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecentChangesAction.cs" company="Jakob Christensen">
//   Copyright (C) 2009 Jakob Christensen
// </copyright>
// <summary>
//   Handles Find Text action, see Actions.xml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AgentJohnson.RecentChanges
{
  using System.Windows.Forms;
  using JetBrains.ActionManagement;
  using JetBrains.Application;
  using JetBrains.IDE;
  using JetBrains.ProjectModel;
  using JetBrains.Util;

  /// <summary>
  /// Handles Find Text action, see Actions.xml
  /// </summary>
  [ActionHandler("RecentChanges")]
  public class RecentChangesAction : IActionHandler
  {
    #region Implemented Interfaces

    #region IActionHandler

    /// <summary>
    /// Executes action. Called after Update, that set ActionPresentation.Enabled to true.
    /// </summary>
    /// <param name="context">
    /// The Data Context
    /// </param>
    /// <param name="nextExecute">
    /// delegate to call
    /// </param>
    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
      var solution = context.GetData(DataConstants.SOLUTION);
      if (solution == null)
      {
        return;
      }

      Execute(solution, context);
    }

    /// <summary>
    /// Updates action visual presentation. If presentation.Enabled is set to false, Execute
    /// will not be called.
    /// </summary>
    /// <param name="context">
    /// The Data Context
    /// </param>
    /// <param name="presentation">
    /// presentation to update
    /// </param>
    /// <param name="nextUpdate">
    /// The delegate to call
    /// </param>
    /// <returns>
    /// <c>True</c>, if successful.
    /// </returns>
    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
      return context.CheckAllNotNull(DataConstants.SOLUTION);
    }

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Executes the specified solution.
    /// </summary>
    /// <param name="solution">
    /// The solution.
    /// </param>
    /// <param name="context">
    /// The context.
    /// </param>
    private static void Execute(ISolution solution, IDataContext context)
    {
      var form = new RecentChanges();

      form.LoadChanges(solution);

      var result = form.ShowDialog();
      if (result != DialogResult.OK)
      {
        return;
      }

      var textControl = context.GetData(DataConstants.TEXT_CONTROL);
      if (textControl == null)
      {
        return;
      }

      var offset = textControl.CaretModel.Offset;

      using (CommandCookie.Create(string.Format("Context Action RecentChanges")))
      {
        using (var cookie = textControl.Document.EnsureWritable())
        {
          if (cookie.EnsureWritableResult != EnsureWritableResult.SUCCESS)
          {
            return;
          }

          textControl.Document.InsertText(offset, form.SelectedText);

          textControl.CaretModel.MoveTo(offset + form.SelectedText.Length);
        }
      }
    }

    #endregion
  }
}