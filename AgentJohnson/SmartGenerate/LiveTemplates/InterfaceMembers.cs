namespace AgentJohnson.SmartGenerate.LiveTemplates
{
  using System.Collections.Generic;
  using JetBrains.ReSharper.Psi.CSharp.Tree;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// </summary>
  [LiveTemplate("Interface member", "Generate interface member")]
  public class InterfaceMembers : ILiveTemplate
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

      IElement interfaceDeclaration = element.GetContainingElement(typeof(IInterfaceDeclaration), true);
      if (interfaceDeclaration == null)
      {
        return null;
      }

      IElement memberDeclaration = element.GetContainingElement(typeof(ITypeMemberDeclaration), true);
      if (memberDeclaration != null && !(memberDeclaration is IInterfaceDeclaration))
      {
        return null;
      }

      LiveTemplateItem liveTemplateItem = new LiveTemplateItem
      {
        MenuText = "Interface member",
        Description = "Interface member",
        Shortcut = "Interface member"
      };

      return new List<LiveTemplateItem>
      {
        liveTemplateItem
      };
    }

    #endregion
  }
}