using JetBrains.ProjectModel;
using JetBrains.ReSharper;
using JetBrains.ActionManagement;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.TextControl;

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
      if (!context.CheckAllNotNull(DataConstants.SOLUTION)){
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
      ITextControl textcontrol = context.GetData(DataConstants.TEXT_CONTROL);
      if(textcontrol == null){
        return;
      }

      Refactoring refactoring = new Refactoring(solution, textcontrol);

      refactoring.Execute();
    }
  }
}