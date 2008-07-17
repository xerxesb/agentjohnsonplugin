using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// </summary>
  [SmartGenerate(Priority=0)]
  public class GenerateStringExpression : SmartGenerateBase {
    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="context">The context.</param>
    /// <param name="element">The element.</param>
    /// <returns>The items.</returns>
    protected override void GetItems(ISolution solution, IDataContext context, IElement element) {
      IExpression expression = element.GetContainingElement(typeof(IExpression), false) as IExpression;
      while (expression != null) {
        IType type = expression.Type();

        string typeName = type.GetPresentableName(element.Language);

        if (typeName == "string") {
          AddMenuItem("Surround with 'string.IsNullOrEmpty'", "3D13FE3E-7004-42B0-B205-2881C5ADBAD2", expression.GetTreeTextRange());
          return;
        }

        expression = expression.GetContainingElement(typeof(IExpression), false) as IExpression;
      }
    }

    #endregion
  }
}