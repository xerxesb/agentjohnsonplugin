namespace AgentJohnson.SmartGenerate.LiveTemplates
{
  using System.Collections.Generic;
  using JetBrains.ReSharper.Psi;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// </summary>
  [LiveTemplate("Body in method that returns a type", "Generates a body in method that returns a type")]
  public class MethodBody : ILiveTemplate
  {
    #region Public methods

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The items.</returns>
    public IEnumerable<LiveTemplateItem> GetItems(SmartGenerateParameters parameters)
    {
      IElement element = parameters.Element;

      IMethodDeclaration methodDeclaration = element.GetContainingElement(typeof(IMethodDeclaration), true) as IMethodDeclaration;
      if (methodDeclaration == null)
      {
        return null;
      }

      IBlock body = methodDeclaration.Body;
      if (body == null || body.Statements.Count > 0)
      {
        return null;
      }

      string name = methodDeclaration.DeclaredName;
      if (string.IsNullOrEmpty(name))
      {
        return null;
      }

      IMethod method = methodDeclaration as IMethod;
      if (method == null)
      {
        return null;
      }

      IType returnType = method.ReturnType;
      string returnTypeName = returnType.GetPresentableName(element.Language);
      if (returnTypeName == "void" || string.IsNullOrEmpty(returnTypeName))
      {
        return null;
      }

      LiveTemplateItem liveTemplateItem = new LiveTemplateItem
      {
        MenuText = string.Format("Body in method that returns '{0}'", returnTypeName),
        Description = string.Format("Body in method that returns '{0}'", returnTypeName),
        Shortcut = string.Format("Body in method that returns '{0}'", returnTypeName)
      };

      return new List<LiveTemplateItem>
      {
        liveTemplateItem
      };
    }

    #endregion
  }
}