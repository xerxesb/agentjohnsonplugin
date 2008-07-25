using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate {
  /// <summary>
  /// Defines the generate boolean expression class.
  /// </summary>
  [SmartGenerate("Surround with 'if'", "Surrounds the boolean expression with 'if'.", Priority=-20)]
  public class GenerateBooleanExpression : SmartGenerateBase {
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

        if (typeName == "bool") {
          AddMenuItem("Surround with 'if'", "FA4B31AF-393D-44DB-93D3-F7E48BF97C53", expression.GetTreeTextRange());
          return;
        }

        expression = expression.GetContainingElement(typeof(IExpression), false) as IExpression;
      }
    }

    #endregion
  }
}