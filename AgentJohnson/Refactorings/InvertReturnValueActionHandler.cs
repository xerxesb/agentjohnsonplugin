using JetBrains.ActionManagement;
using JetBrains.IDE;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;

namespace AgentJohnson.Refactorings {
  /// <summary>
  /// </summary>
  [ActionHandler("AgentJohnson.InvertReturnValue")]
  public class InvertReturnValueActionHandler : ActionHandlerBase {
    /// <summary>
    /// Updates the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns></returns>
    protected override bool Update(IDataContext context) {
      if(!context.CheckAllNotNull(DataConstants.SOLUTION)) {
        return false;
      }

      IElement element = GetElementAtCaret(context);
      if(element == null) {
        return false;
      }

      return InvertReturnValueRefactoring.IsAvailable(element);
    }

    /// <summary>
    /// Executes action. Called after Update, that set ActionPresentation.Enabled to true.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    protected override void Execute(ISolution solution, IDataContext context) {
      ITextControl textControl = context.GetData(DataConstants.TEXT_CONTROL);
      if(textControl == null) {
        return;
      }

      InvertReturnValueRefactoring invertReturnValueRefactoring = new InvertReturnValueRefactoring(solution, textControl);

      invertReturnValueRefactoring.Execute();
    }
  }
}