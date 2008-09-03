using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace AgentJohnson.SmartGenerate.Generators {
  /// <summary>
  /// </summary>
  [SmartGenerate("Generate 'return'", "Generates a 'return' statement.", Priority = 1000)]
  public class Return : SmartGenerateBase {
    #region Protected methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">The get menu items parameters.</param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters) {
      IElement element = smartGenerateParameters.Element;

      if(!IsAfterLastStatement(element)) {
        return;
      }

      IBlock block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if(block == null) {
        return;
      }

      IBlock body = null;
      string typeName = string.Empty;
      IType returnType = null;

      IFunction function = block.GetContainingTypeMemberDeclaration() as IFunction;
      if(function != null) {
        returnType = function.ReturnType;
        typeName = returnType.GetPresentableName(element.Language);

        IFunctionDeclaration functionDeclaration = function as IFunctionDeclaration;
        if(functionDeclaration != null) {
          body = functionDeclaration.Body;
        }
      }
      else {
        IProperty property = block.GetContainingTypeMemberDeclaration() as IProperty;
        if(property != null) {
          IAccessorDeclaration accessorDeclaration = element.GetContainingElement(typeof(IAccessorDeclaration), true) as IAccessorDeclaration;

          if(accessorDeclaration != null && accessorDeclaration.Kind == AccessorKind.GETTER) {
            returnType = property.Type;
            typeName = returnType.GetPresentableName(element.Language);
            body = accessorDeclaration.Body;
          }
        }
      }

      // return;
      if(string.IsNullOrEmpty(typeName) || typeName == "void") {
        if(body != block && body != null) {
          AddMenuItem("return;", "19B0E24A-C3C3-489A-BF20-122C5114D7FF");
        }
      }
      else if(typeName == "bool") {
        AddMenuItem("Return 'true'", "459C8B38-0048-43DF-9279-3E946A3A65F2");
        AddMenuItem("Return 'false'", "9F342BE4-4A55-48FF-BECF-A67C7D79BF76");
      }
      else {
        AddMenuItem("Return a value", "39530254-7198-4A3C-B528-6160324E9792");

        if(returnType != null && returnType.IsReferenceType()) {
          AddMenuItem("Return 'null'", "D34007F3-C131-46F4-96B7-8D2654727D0B");
        }
      }
    }

    #endregion
  }
}