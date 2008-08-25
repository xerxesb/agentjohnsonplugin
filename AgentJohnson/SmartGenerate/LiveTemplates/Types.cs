using System.Collections.Generic;
using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.LiveTemplates {
  /// <summary>
  /// </summary>
  [LiveTemplate("Type", "Generate type")]
  public class Types : ILiveTemplate {
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
      IElement classLikeDeclaration = element.GetContainingElement(typeof(IClassLikeDeclaration), true);
      if(classLikeDeclaration != null) {
        return null;
      }

      IEnumDeclaration enumDecl = element.GetContainingElement(typeof(IEnumDeclaration), true) as IEnumDeclaration;
      if(enumDecl != null) {
        return null;
      }

      IElement namespaceDeclaration = element.GetContainingElement(typeof(INamespaceDeclaration), true);
      if(namespaceDeclaration == null) {
        return null;
      }

      LiveTemplateItem liveTemplateItem = new LiveTemplateItem {
        MenuText = "Type",
        Description = "Type",
        Shortcut = "Type"
      };

      return new List<LiveTemplateItem> {
        liveTemplateItem
      };
    }

    #endregion
  }
}