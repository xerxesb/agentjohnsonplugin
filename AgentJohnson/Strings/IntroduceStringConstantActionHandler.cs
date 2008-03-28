using JetBrains.ProjectModel;
using JetBrains.ActionManagement;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.TextControl;

namespace AgentJohnson.Strings {
  /// <summary>
  /// </summary>
  [ActionHandler("AgentJohnson.IntroduceStringConstant")]
  public class IntroduceStringConstantActionHandler : ActionHandlerBase {
    /// <summary>
    /// Updates the specified context.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns></returns>
    protected override bool Update(IDataContext context) {
      if(!context.CheckAllNotNull(JetBrains.IDE.DataConstants.SOLUTION)) {
        return false;
      }

      IElement element = GetElementAtCaret(context);
      if(element == null){
        return false;
      }

      return Refactoring.IsAvailable(element);
    }

    /// <summary>
    /// Executes action. Called after Update, that set ActionPresentation.Enabled to true.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    protected override void Execute(ISolution solution, IDataContext context) {
      ITextControl textControl = context.GetData(JetBrains.IDE.DataConstants.TEXT_CONTROL);
      if(textControl == null){
        return;
      }

      Refactoring refactoring = new Refactoring(solution, textControl);

      refactoring.Execute();
    }
  }
}