using System.Collections.Generic;
using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.LiveTemplates {
  /// <summary>
  /// </summary>
  [LiveTemplate("Interface member", "Generate interface member")]
  public class InterfaceMembers : ILiveTemplate {
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
      IElement interfaceDeclaration = element.GetContainingElement(typeof(IInterfaceDeclaration), true);
      if(interfaceDeclaration == null) {
        return null;
      }

      IElement memberDeclaration = element.GetContainingElement(typeof(ITypeMemberDeclaration), true);
      if(memberDeclaration != null && !(memberDeclaration is IInterfaceDeclaration)) {
        return null;
      }

      LiveTemplateItem liveTemplateItem = new LiveTemplateItem {
        MenuText = "Interface member",
        Description = "Interface member",
        Shortcut = "Interface member"
      };

      return new List<LiveTemplateItem> {
        liveTemplateItem
      };
    }

    #endregion
  }
}