namespace AgentJohnson.SmartGenerate.Generators
{
  using System;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;
  using JetBrains.ReSharper.Psi.Util;

  /// <summary>
  /// </summary>
  [SmartGenerate("Generate 'return'", "Generates a 'return' statement.", Priority = 1000)]
  public class Return : SmartGenerateHandlerBase
  {
    #region Protected methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">The get menu items parameters.</param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters)
    {
      IElement element = smartGenerateParameters.Element;

      if (!StatementUtil.IsAfterLastStatement(element))
      {
        return;
      }

      IBlock block = element.GetContainingElement(typeof(IBlock), true) as IBlock;
      if (block == null)
      {
        return;
      }

      IBlock body = null;
      string typeName = string.Empty;
      IType returnType = null;

      IFunctionDeclaration functionDeclaration = block.GetContainingTypeMemberDeclaration() as IFunctionDeclaration;
      if (functionDeclaration != null)
      {
        IFunction function = functionDeclaration.DeclaredElement;
        if (function == null)
        {
          return;
        }

        returnType = function.ReturnType;
        typeName = returnType.GetPresentableName(element.Language);

        IMethodDeclaration methodDeclaration = functionDeclaration as IMethodDeclaration;
        if (methodDeclaration != null)
        {
          body = methodDeclaration.Body;
        }
      }
      else
      {
        IPropertyDeclaration propertyDeclaration = block.GetContainingTypeMemberDeclaration() as IPropertyDeclaration;
        if (propertyDeclaration != null)
        {
          IAccessorDeclaration accessorDeclaration = element.GetContainingElement(typeof(IAccessorDeclaration), true) as IAccessorDeclaration;

          if (accessorDeclaration != null && accessorDeclaration.Kind == AccessorKind.GETTER)
          {
            returnType = propertyDeclaration.Type;
            typeName = returnType.GetPresentableName(element.Language);
            body = accessorDeclaration.Body;
          }
        }
      }

      // return;
      if (string.IsNullOrEmpty(typeName) || typeName == "void")
      {
        if (body != block && body != null)
        {
          this.AddAction("return;", "19B0E24A-C3C3-489A-BF20-122C5114D7FF");
        }
      }
      else if (typeName == "bool")
      {
        this.AddAction("Return 'true'", "459C8B38-0048-43DF-9279-3E946A3A65F2");
        this.AddAction("Return 'false'", "9F342BE4-4A55-48FF-BECF-A67C7D79BF76");
      }
      else
      {
        this.AddAction("Return a value", "39530254-7198-4A3C-B528-6160324E9792");

        if (returnType != null && returnType.IsReferenceType())
        {
          this.AddAction("Return 'null'", "D34007F3-C131-46F4-96B7-8D2654727D0B");
        }
      }
    }

    #endregion
  }
}