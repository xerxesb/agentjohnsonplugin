using System.Collections.Generic;
using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.LiveTemplates {
  /// <summary>
  /// </summary>
  [LiveTemplate("Outside namespace", "Executes a Live Template outside a namespace.")]
  public class OutsideNamespace : ILiveTemplate {
    #region Public methods

    /// <summary>
    /// Gets the name of the template.
    /// </summary>
    /// <param name="solution">The solution.</param>
    /// <param name="dataContext">The data context.</param>
    /// <param name="previousStatement">The previous statement.</param>
    /// <param name="element">The element.</param>
    /// <returns></returns>
    public IEnumerable<LiveTemplateItem> GetItems(ISolution solution, IDataContext dataContext, IStatement previousStatement, IElement element) {
      IElement namespaceDeclaration = element.GetContainingElement(typeof(INamespaceDeclaration), true);
      if(namespaceDeclaration != null) {
        return null;
      }

      LiveTemplateItem liveTemplateItem = new LiveTemplateItem {
        MenuText = "Outside namespace",
        Description = "Outside namespace",
        Shortcut = "Outside namespace"
      };

      return new List<LiveTemplateItem> {
        liveTemplateItem
      };
    }

    #endregion
  }
}