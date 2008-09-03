using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.Generators {
  /// <summary>
  /// </summary>
  [SmartGenerate("Surround with 'string.IsNullOrEmpty'", "Surrounds the string expression with 'string.IsNullOrEmpty'.", Priority = -20)]
  public class StringExpression : SmartGenerateBase {
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

        if(typeName == "string") {
          AddMenuItem("Surround with 'string.IsNullOrEmpty'", "3D13FE3E-7004-42B0-B205-2881C5ADBAD2", expression.GetTreeTextRange());
          return;
        }

        expression = expression.GetContainingElement(typeof(IExpression), false) as IExpression;
      }
    }

    #endregion
  }
}