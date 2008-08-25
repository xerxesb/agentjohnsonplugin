using System.Collections.Generic;
using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.LiveTemplates {
  /// <summary>
  /// </summary>
  [LiveTemplate("Method body", "Generate Method body")]
  public class MethodBody: ILiveTemplate {
    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="dataContext">The data context.</param>
    /// <param name="previousStatement">The previous statement.</param>
    /// <param name="element">The element.</param>
    /// <returns>The items.</returns>
    public IEnumerable<LiveTemplateItem> GetItems(ISolution solution, IDataContext dataContext, IStatement previousStatement, IElement element) {
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

      IType returnType = method.ReturnType;
      string returnTypeName = returnType.GetPresentableName(element.Language);
      if(returnTypeName == "void" || string.IsNullOrEmpty(returnTypeName)) {
        return null;
      }

      LiveTemplateItem liveTemplateItem = new LiveTemplateItem {
        MenuText = "Method body",
        Description = "Method body",
        Shortcut = "Method body"
      };

      return new List<LiveTemplateItem> {
        liveTemplateItem
      };
    }

    #endregion
  }
}