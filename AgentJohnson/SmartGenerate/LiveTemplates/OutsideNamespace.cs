namespace AgentJohnson.SmartGenerate.LiveTemplates
{
  using System.Collections.Generic;
  using JetBrains.ReSharper.Psi.Tree;

  /// <summary>
  /// </summary>
  [LiveTemplate("Outside namespace", "Executes a Live Template outside a namespace.")]
  public class OutsideNamespace : ILiveTemplate
  {
    #region Public methods

    /// <summary>
    /// Gets the name of the template.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>The items.</returns>
    public IEnumerable<LiveTemplateItem> GetItems(SmartGenerateParameters parameters)
    {
      IElement element = parameters.Element;

      IElement namespaceDeclaration = element.GetContainingElement(typeof(INamespaceDeclaration), true);
      if (namespaceDeclaration != null)
      {
        return null;
      }

      LiveTemplateItem liveTemplateItem = new LiveTemplateItem
      {
        MenuText = "Outside namespace",
        Description = "Outside namespace",
        Shortcut = "Outside namespace"
      };

      return new List<LiveTemplateItem>
      {
        liveTemplateItem
      };
    }

    #endregion
  }
}