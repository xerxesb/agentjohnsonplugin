using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;

namespace AgentJohnson.SmartGenerate.Generators {
  /// <summary>
  /// </summary>
  [SmartGenerate("Generate method body", "Generates the body of a method with a return value.", Priority = 0)]
  public class MethodBody : SmartGenerateBase {
    #region Protected methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="smartGenerateParameters">The get menu items parameters.</param>
    protected override void GetItems(SmartGenerateParameters smartGenerateParameters) {
      IElement element = smartGenerateParameters.Element;

      IMethodDeclaration methodDeclaration = element.GetContainingElement(typeof(IMethodDeclaration), true) as IMethodDeclaration;
      if(methodDeclaration == null) {
        return;
      }

      IBlock body = methodDeclaration.Body;
      if(body == null || body.Statements.Count > 0) {
        return;
      }

      string name = methodDeclaration.ShortName;
      if(string.IsNullOrEmpty(name)) {
        return;
      }

      IMethod method = methodDeclaration as IMethod;
      if(method == null) {
        return;
      }

      IType returnType = method.ReturnType;
      string returnTypeName = returnType.GetPresentableName(element.Language);
      if(returnTypeName == "void" || string.IsNullOrEmpty(returnTypeName)) {
        return;
      }

      if(returnType.IsReferenceType()) {
        AddMenuItem("Declare and return variable", "D00FD7E4-EE48-40F0-A126-D1E8AE8C031E", returnTypeName);
      }
      else {
        AddMenuItem("Declare and return variable", "1E9A90C2-2FB9-451F-8566-BA59AAB707DF", returnTypeName);
      }
    }

    #endregion
  }
}