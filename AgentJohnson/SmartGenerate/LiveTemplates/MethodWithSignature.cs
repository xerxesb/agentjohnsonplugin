using System.Collections.Generic;
using System.Text;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.LiveTemplates {
  /// <summary>
  /// </summary>
  [LiveTemplate("Method with signature", "Generate body in a method with a signature")]
  public class MethodWithSignature : ILiveTemplate {
    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The items.</returns>
    public IEnumerable<LiveTemplateItem> GetItems(SmartGenerateParameters parameters) {
      IElement element = parameters.Element;

      IMethodDeclaration methodDeclaration = element.GetContainingElement(typeof(IMethodDeclaration), true) as IMethodDeclaration;
      if(methodDeclaration == null) {
        return null;
      }

      IBlock body = methodDeclaration.Body;
      if(body == null || body.Statements.Count > 0) {
        return null;
      }

      string name = methodDeclaration.ShortName;
      if(string.IsNullOrEmpty(name)) {
        return null;
      }

      IMethod method = methodDeclaration as IMethod;
      if(method == null) {
        return null;
      }

      StringBuilder signatureBuilder = new StringBuilder();
      bool first = true;

      foreach(IParameter parameter in method.Parameters) {
        if(!first) {
          signatureBuilder.Append(", ");
        }
        first = false;

        signatureBuilder.Append(parameter.Type.GetLongPresentableName(element.Language));
      }

      string signature = signatureBuilder.ToString();

      return new List<LiveTemplateItem> {
        new LiveTemplateItem {
          MenuText = string.Format("Body in method with signature ({0})", signature),
          Description = string.Format("Body in method with signature ({0})", signature),
          Shortcut = string.Format("Body in method with signature ({0})", signature)
        }
      };
    }

    #endregion
  }
}