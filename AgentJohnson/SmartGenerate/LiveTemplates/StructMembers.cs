using System.Collections.Generic;
using JetBrains.ActionManagement;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace AgentJohnson.SmartGenerate.LiveTemplates {
  /// <summary>
  /// </summary>
  [LiveTemplate("Struct member", "Generate struct member")]
  public class StructMembers : ILiveTemplate {
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
      IStructDeclaration structDeclaration = element.GetContainingElement(typeof(IStructDeclaration), true) as IStructDeclaration;
      if(structDeclaration == null) {
        return null;
      }

      IElement memberDeclaration = element.GetContainingElement(typeof(IClassMemberDeclaration), true);
      if(memberDeclaration != null && !(memberDeclaration is IStructDeclaration)) {
        return null;
      }

      string modifier = ModifierUtil.GetModifier(element, structDeclaration);

      LiveTemplateItem liveTemplateItem = new LiveTemplateItem {
        MenuText = "Struct member",
        Description = "Struct member",
        Shortcut = "Struct member"
      };

      liveTemplateItem.Variables["Modifier"] = modifier;

      return new List<LiveTemplateItem> {
        liveTemplateItem
      };
    }

    #endregion
  }
}