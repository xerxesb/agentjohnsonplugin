using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.Generators {
  /// <summary>
  /// Defines the generate boolean expression class.
  /// </summary>
  [SmartGenerate("Surround with 'if'", "Surrounds the boolean expression with 'if'.", Priority = -20)]
  public class BooleanExpression : SmartGenerateBase {
    #region Protected methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">The get menu items parameters.</param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters) {
      IElement element = smartGenerateParameters.Element;

      IExpression expression = element.GetContainingElement(typeof(IExpression), false) as IExpression;
      while(expression != null) {
        IType type = expression.Type();

        string typeName = type.GetPresentableName(element.Language);

        if(typeName == "bool") {
          AddMenuItem("Surround with 'if'", "FA4B31AF-393D-44DB-93D3-F7E48BF97C53", expression.GetTreeTextRange());
          return;
        }

        expression = expression.GetContainingElement(typeof(IExpression), false) as IExpression;
      }
    }

    #endregion
  }
}